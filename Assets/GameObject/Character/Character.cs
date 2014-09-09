using UnityEngine;
using System;
using System.Collections;

public partial class Character : MonoBehaviour 
{
	public CharacterType type = CharacterType.NONE;

	public string owner;

	#region movement
	private bool m_Floating = false;
	public bool floating { get {return m_Floating; }}
	
	private float m_JumpCooltime = 0.0f;
	public float jumpVelocity = 10.0f;
	public float jumpCooldown = 1.0f;
	public float moveForce = 10.0f;

	private AttackData m_LastAttackData;
	public AttackData lastAttackData { get { return m_LastAttackData; }}

	public int direction {
		get { return transform.rotation.y > 0.5f ? -1 : 1; }
		set {
			if (direction == value) return;

			var _rotation = transform.localRotation;
			
			if (value == 1) 
			{
				_rotation.y = 0;
			}
			else if (value == -1)
			{
				_rotation.y = 180;
			}
			
			transform.localRotation = _rotation;
		}
	}
	#endregion

	#region pose
	private bool m_IsStanding = true;
	public bool isStanding { get { return m_IsStanding; }}
	public bool isCrouching { get { return ! m_IsStanding; }}

	public LayerMask terrainMask;
	#endregion

	#region life_state
	public int hpMax = 1;
	private HasHP m_Hp;

	public float hitCooldown = 0.5f;
	public Vector2 hitForce = new Vector2(10.0f, 5.0f);
	
	public bool hitEnabled {
		get { return damageDetector.enabled; }
		set { damageDetector.enabled = value; }
	}
	
	private bool m_Dead = false;
	public bool isDead {
		get { return m_Dead; }
		private set { m_Dead = value; }
	}
	
	public Vector2 deadForce;
	public float deadDelay = 0.5f;
	#endregion

	#region weapon
	private Weapon m_Weapon;
	public WeaponEquip weaponEquip { get; private set; }

	public Weapon weapon {
		get { return m_Weapon; }
		set {
			var _old = m_Weapon;
			var _oldEquip = weaponEquip;

			m_Weapon = value;

			var _aimTemp = aim;
			m_Aim = -1; // invalidate aim
			aim = _aimTemp;

			if (m_Weapon == null) 
			{
				m_NetworkAnimator.SetTrigger(CharacterAnimationTrigger.UNEQUIP);
			}
			else 
			{
				m_Weapon.owner = gameObject;
				m_Weapon.transform.parent = weaponPivot.transform;
				m_Weapon.transform.localPosition = Vector3.zero;
				m_Weapon.transform.localEulerAngles = Vector3.zero;
				m_Weapon.postOutOfAmmo += ListenOutOfAmmo;
				m_Weapon.postCooldown += ListenWeaponCooldown;

				var _animationGroup = WeaponHelper.GetTrigger(m_Weapon.animationGroup);
				animator.SetTrigger(CharacterAnimationTrigger.ArmWeaponEquip(_animationGroup));
				animator.SetTrigger(CharacterAnimationTrigger.UpperWeaponEquip(_animationGroup));

				var _weaponEquipPrf = Database.Weapon[m_Weapon.type].weaponEquipPrf;
				weaponEquip = ((GameObject) Instantiate(_weaponEquipPrf)).GetComponent<WeaponEquip>();
				weaponEquip.transform.parent = weaponEquipPivot.transform;
				weaponEquip.transform.localPosition = Vector3.zero;
				weaponEquip.transform.localEulerAngles = Vector3.zero;
				
				if (IsMine() && IsNetworkEnabled())
				{
					m_Weapon.networkView.viewID = Network.AllocateViewID();
					m_Weapon.networkView.enabled = true;
					networkView.RPC("Character_SetWeapon", 
					                RPCMode.Others, 
					                m_Weapon.networkView.viewID, 
					                (int) m_Weapon.type);
				}
			}

			if (postWeaponChanged != null)
				postWeaponChanged(this, _old);

			if (_old) 
				Destroy(_old.gameObject);

			//if (_oldEquip)
			//	Destroy(_oldEquip.gameObject, 2f);
		}
	}

	public GameObject weaponPivot;
	public GameObject weaponEquipPivot;

	[RPC]
	private void Character_SetWeapon(NetworkViewID _viewID, int _weapon)
	{
		var _weaponType = WeaponHelper.Convert(_weapon);
		if (_weaponType == WeaponType.NONE) 
			return;

		var _weaponData = Database.Weapon[_weaponType];
		var _weaponObj = GameObject.Instantiate(_weaponData) as GameObject;
		weapon = _weaponObj.GetComponent<Weapon>();
		weapon.networkView.enabled = true;
		weapon.networkView.viewID = _viewID;
	}

	private float m_Aim = 90;
	public float aim { 
		get { return m_Aim; }
		set { 
			if (isDead) return;
			if (m_Aim == value) return;

			m_Aim = value;
			
			var _weaponAngle = weaponPivot.transform.eulerAngles;
			_weaponAngle.z = m_Aim - 90;
			weaponPivot.transform.eulerAngles = _weaponAngle;
			weaponEquipPivot.transform.eulerAngles = _weaponAngle;
			
			animator.SetFloat("aim", m_Aim);

			if (crossHair) 
			{
				var _angle = crossHair.transform.localEulerAngles;
				_angle.z = m_Aim - 90;
				crossHair.transform.localEulerAngles = _angle;
			}
		}
	}

	public float aimSpeed = 90f;

	#endregion

	#region components
	public CharacterRenderer renderer_;

	public Animator animator;
	private NetworkAnimator m_NetworkAnimator;
	public CharacterAnimationEventer m_AnimationEventor;

	private InterpolatePosition m_InterpolatePosition;

	public GameObject crossHair;
	
	// detector
	public CrateDetector crateDetector;
	public DamageDetector damageDetector;
	public LayerDetector terrainDetector;
	
	public Collider2D deadZoneCollider;
	#endregion

	#region events
	public delegate void PostWeaponChanged(Character _character, Weapon _old);
	public event PostWeaponChanged postWeaponChanged;
	public event Action<Character> postDead;	
	#endregion

	#region network
	public bool IsMine()
	{
		return owner == Network.player.guid;
	}

	public bool IsNetworkEnabled() 
	{
		return networkView.enabled && Network.peerType != NetworkPeerType.Disconnected;
	}

	#endregion

	#region effects
	public GameObject effectDeadPrf;
	public Vector3 effectDeadOffset;
	#endregion

	void Awake () {
		m_Hp = GetComponent<HasHP>();
		m_Hp.hp = hpMax;
		m_Hp.postDead = Die;

		// components
		m_NetworkAnimator = GetComponent<NetworkAnimator>();
		m_InterpolatePosition = gameObject.AddComponent<InterpolatePosition>();

		m_AnimationEventor.postThrowAway += ListenAnimationEventThrowAway;

		// detector
		crateDetector.doObtain = Obtain;
		damageDetector.doDamage = Hit;
		
		terrainDetector.postDetect = (Collision) =>
		{
			m_Floating = false;
			terrainDetector.gameObject.SetActive(false);
		};
	}
	
	void DestroySelf()
	{
		if (networkView.enabled)
		{
			Network.Destroy(networkView.viewID);
		}
		else
		{
			GameObject.Destroy(gameObject);
		}
	}

	void Update() 
	{
		m_JumpCooltime -= Time.deltaTime;
		animator.SetFloat("speed_x", direction * rigidbody2D.velocity.x);
		animator.SetFloat("velocity_y", rigidbody2D.velocity.y);

		var _rayResult = Physics2D.Raycast(transform.position, -Vector2.up, 1f, terrainMask);

		if (_rayResult) {
			float _tilt;
			if (direction > 0) {
				_tilt = Mathf.Atan2(_rayResult.normal.y, _rayResult.normal.x);
			} else {
				_tilt = Mathf.Atan2(_rayResult.normal.y, -_rayResult.normal.x);
			}
			
			_tilt *= Mathf.Rad2Deg;
			
			animator.SetFloat("tilt", _tilt);
		}
	}

	public void Stand()
	{
		m_IsStanding = true;
		m_NetworkAnimator.SetTrigger("stand_lower");

		if (IsMine() && IsNetworkEnabled ())
			networkView.RPC ("StandRPC", RPCMode.Others);
	}

	[RPC]
	void StandRPC()
	{
		Stand();
	}

	public void Crouch()
	{
		m_IsStanding = false;
		m_NetworkAnimator.SetTrigger("crouch_lower");

		if (IsMine() && IsNetworkEnabled ())
			networkView.RPC ("CrouchRPC", RPCMode.Others);
	}
	
	[RPC]
	void CrouchRPC()
	{
		Crouch();
	}

	public bool movable { 
		get { return ! isDead; }
	}
	
	public void Move(float _direction)
	{
		rigidbody2D.AddForce(_direction * moveForce * Vector3.right);
	}
	
	public bool jumpable {
		get { return ! m_Floating && m_JumpCooltime <= 0 && ! isDead; }
	}
	
	public void Jump()
	{
		m_Floating = true;
		m_JumpCooltime = jumpCooldown;
		rigidbody2D.velocity += new Vector2(0, jumpVelocity);
		terrainDetector.gameObject.SetActive(true);
		m_NetworkAnimator.SetTrigger(CharacterAnimationTrigger.JUMP_LOWER);
		m_NetworkAnimator.SetTrigger(CharacterAnimationTrigger.JUMP_UPPER);
	}
	
	public void ChangeAim(float _direction)
	{
		aim += _direction * aimSpeed;
	}

	public bool shootable {
		get {
			return weapon != null && weapon.IsShootable()
				&& ! isDead; 
		}
	}
	
	public void Shoot() {
		weapon.Shoot();
		m_NetworkAnimator.SetTrigger ("shoot");
	}
	
	void EnableHit() {
		hitEnabled = true;
	}

	public void Hit(AttackData _attackData) 
	{
//		Debug.Log("Hit");

		if (! hitEnabled) return;

		if (IsNetworkEnabled())
		{
			networkView.RPC("Character_Hit", RPCMode.All, _attackData.Serialize());
		}
		else
		{
			HitLocal(_attackData);
		}
	}
	
	void HitLocal(AttackData _attackData)
	{
//		Debug.Log("Hit Local");
		hitEnabled = false;
		CancelInvoke("EnableHit");
		Invoke("EnableHit", hitCooldown);
		
		m_LastAttackData = _attackData;
		
		var _direction = Mathf.Sign(_attackData.velocity.x);
		
		rigidbody2D.AddForce(new Vector2(_direction * hitForce.x, hitForce.y));
		
		m_Hp.damage(_attackData);
		
		if (m_Hp > 0) 
			m_NetworkAnimator.SetTrigger("Hit");
	}

	[RPC]
	void Character_Hit(string _attackDataSerial)
	{
//		Debug.Log("Hit Network");
		AttackData _attackData = AttackData.Deserialize(_attackDataSerial);
		HitLocal(_attackData);
	}

	void Die() {
		if (isDead) {
			Debug.LogWarning("Trying to kill already dead character. Ignore.");
			return;
		}

		CancelInvoke("EnableHit");
		
		isDead = true;
		hitEnabled = false;
		
		var _deadForce = deadForce;
		_deadForce.x *= -direction;
		rigidbody2D.velocity = new Vector2(0, 0);
		rigidbody2D.AddForce(_deadForce);
		
		m_NetworkAnimator.SetTrigger("dead_lower");
		m_NetworkAnimator.SetTrigger("dead_upper");
		
		var _effectDead = (GameObject) Instantiate (effectDeadPrf, transform.position, transform.rotation);
		_effectDead.transform.Translate (effectDeadOffset);

		if (postDead != null) postDead(this);

		Invoke("DestroySelf", deadDelay);
	}
	
	void Obtain(Crate _crate) 
	{
		if ( IsNetworkEnabled() && ! IsMine()) 
			return;
		
		if (_crate.empty) 
			return;
		
		var _weapon = (GameObject) Instantiate(Resources.Load(_crate.weapon)) ;
		weapon = _weapon.GetComponent<Weapon>();
	}

	private void ListenOutOfAmmo(Weapon _weapon)
	{
		// note: Unequip is called by animator.
		m_NetworkAnimator.SetTrigger ("throw_away");
	}
	
	private void ListenWeaponCooldown(Weapon _weapon)
	{
		m_NetworkAnimator.SetTrigger ("rest");
	}

	public void Unequip()
	{
		weapon = null;
		weaponEquip = null;
	}

	void OnSerializeNetworkView(BitStream _stream, NetworkMessageInfo _info) 
	{
		m_InterpolatePosition.OnSerializeNetworkView(_stream, _info);
		
		float _aim = 0;
		int _direction = 0;

		if (_stream.isWriting) 
		{
			_aim = aim;
			_direction = direction;

			_stream.Serialize(ref _aim);
			_stream.Serialize(ref _direction);
		} 
		else 
		{
			_stream.Serialize(ref _aim);
			_stream.Serialize(ref _direction);

			aim = _aim;
			direction = _direction;
		}
	}

}
