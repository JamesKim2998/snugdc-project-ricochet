using UnityEngine;
using System;
using System.Collections;

public enum CharacterType
{
	NONE,
	BLUE,
}

public class Character : MonoBehaviour 
{
	public CharacterType type = CharacterType.NONE;

	#region movement
	private bool m_Floating = false;
	public bool floating { get {return m_Floating; }}
	
	private float m_JumpCooltime = 0.0f;
	public float jumpForce = 10.0f;
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

	public Weapon weapon {
		get { return m_Weapon; }
		set {
			var _old = m_Weapon;

			m_Weapon = value;

			if (m_Weapon != null) {
				m_Weapon.transform.parent = weaponPivot.transform;
				m_Weapon.transform.localPosition = Vector3.zero;
				m_Weapon.transform.localEulerAngles = Vector3.zero;
				m_Weapon.owner = gameObject;
			}

			var _aimTemp = aim;
			m_Aim = -1; // invalidate aim
			aim = _aimTemp;

			if (m_Weapon != null)
			{
				m_Weapon.postOutOfAmmo += ListenOutOfAmmo;
			}
			
			if (m_Weapon == null) 
			{
				m_NetworkAnimator.SetTrigger("unequip");
			}
			else 
			{
				m_NetworkAnimator.SetTrigger("equip_" + m_Weapon.animationGroup);

				if (IsNetworkEnabled())
				{
					m_Weapon.networkView.viewID = Network.AllocateViewID();
					m_Weapon.networkView.enabled = true;
					networkView.RPC("SetWeaponRPC", RPCMode.OthersBuffered, weapon.networkView.viewID, weapon.animationGroup);
				}
			}

			if (postWeaponChanged != null)
				postWeaponChanged(this, _old);

			if (_old != null) 
				Destroy(_old.gameObject);

		}
	}

	public WeaponPivot weaponPivot;

	[RPC]
	private void SetWeaponRPC(NetworkViewID _viewID, string _weapon)
	{
		var _weaponObj = GameObject.Instantiate(Resources.Load(_weapon)) as GameObject;
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

			m_Animator.SetFloat("aim", m_Aim);

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
	private CharacterRenderer m_Renderers;
	public CharacterRenderer renderers { get { return m_Renderers; } }

	private Animator m_Animator;
	private NetworkAnimator m_NetworkAnimator;

	private InterpolatePosition m_InterpolatePosition;

	public GameObject crossHair;
	
	// detector
	public CrateDetector crateDetector;
	public DamageDetector damageDetector;
	public LayerDetector terrainDetector;
	#endregion

	#region events
	public delegate void PostWeaponChanged(Character _character, Weapon _old);
	public event PostWeaponChanged postWeaponChanged;
	public event Action<Character> postDead;	
	#endregion

	#region network
	public bool IsNetworkEnabled() 
	{
		return networkView.isMine && (Network.peerType != NetworkPeerType.Disconnected);
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
		m_Renderers = GetComponent<CharacterRenderer> ();
		m_Animator = GetComponent<Animator>();
		m_NetworkAnimator = GetComponent<NetworkAnimator>();
		m_InterpolatePosition = gameObject.AddComponent<InterpolatePosition>();

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
		m_NetworkAnimator.SetFloat("speed_x", Mathf.Abs(rigidbody2D.velocity.x));
		m_NetworkAnimator.SetFloat("velocity_y", rigidbody2D.velocity.y);
	}

	public void Stand()
	{
		m_IsStanding = true;
		m_NetworkAnimator.SetTrigger("stand_lower");

		if (IsNetworkEnabled ())
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

		if (IsNetworkEnabled ())
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
		rigidbody2D.velocity += new Vector2(0, jumpForce);
		terrainDetector.gameObject.SetActive(true);
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
	
	void Hit(AttackData _attackData) {
		if (! hitEnabled) return;
		hitEnabled = false;
		
		Invoke("EnableHit", hitCooldown);

		m_LastAttackData = _attackData;

		var _direction = Mathf.Sign(_attackData.velocity.x);

		rigidbody2D.AddForce(new Vector2(_direction * hitForce.x, hitForce.y));

		m_Hp.damage(_attackData);
		
		if (m_Hp > 0) {
			m_NetworkAnimator.SetTrigger("Hit");
		}
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
		
		var _effectDead = GameObject.Instantiate (effectDeadPrf, transform.position, transform.rotation) as GameObject;
		_effectDead.transform.Translate (effectDeadOffset);

		if (postDead != null) postDead(this);

		Invoke("DestroySelf", deadDelay);
	}
	
	void Obtain(Crate _crate) 
	{
		if (networkView.enabled && ! networkView.isMine) 
			return;
		
		if (_crate.empty) return;
		
		var _weapon = GameObject.Instantiate(Resources.Load(_crate.weapon)) as GameObject;
		weapon = _weapon.GetComponent<Weapon>();
	}

	private void ListenOutOfAmmo(Weapon _weapon)
	{
		// note: Unequip 은 animator 에서 호출합니다
		m_NetworkAnimator.SetTrigger ("throw_away");
	}

	public void Unequip()
	{
		weapon = null;
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
