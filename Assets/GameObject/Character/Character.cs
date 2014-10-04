using UnityEngine;
using System;
using System.Collections;
using Random = System.Random;

public partial class Character : MonoBehaviour
{
    private static readonly Random s_Random = new Random();

    public int id = 0;

	public CharacterType type = CharacterType.NONE;

	public string ownerPlayer;

	#region movement

    public bool floating { get; private set; }

    private float m_JumpCooltime = 0.0f;
	public float jumpVelocity = 10.0f;
	public float jumpCooldown = 1.0f;
	public float moveForce = 10.0f;

    public AttackData lastAttackData { get; set; }

    public int direction {
		get { return transform.rotation.y > 0.5f ? -1 : 1; }
		set {
			if (direction == value) return;

			var _rotation = transform.localRotation;
			
			if (value == 1) 
				_rotation.y = 0;
			else if (value == -1)
				_rotation.y = 180;
			
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
	private PropertyHP m_Hp;

	public float hitCooldown = 0.5f;
	public Vector2 hitForce = new Vector2(10.0f, 5.0f);

    public SetCounter<int> hitDisabled = new SetCounter<int>();

    public bool isDead { get; private set; }

    public Vector2 deadForce;
	public float deadDelay = 0.5f;
	#endregion

	#region weapon
	private Weapon m_Weapon;
	private WeaponEquip m_WeaponEquip { get; set; }
    private bool m_ShouldAimWeapon = false;

	public Weapon weapon {
		get { return m_Weapon; }
		set {
			var _old = m_Weapon;

		    if (m_Weapon)
		    {
                if (m_Weapon.IsState(Weapon.State.SHOOTING))
                    animator.SetBool("rest", true);
		    }

		    if (m_WeaponEquip)
                ThrowAway();

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
			    m_ShouldAimWeapon = true;
			    m_Weapon.ownerPlayer = ownerPlayer;
				m_Weapon.ownerGameObj = gameObject;
				m_Weapon.transform.parent = weaponPivot.transform;
				m_Weapon.transform.localPosition = Vector3.zero;
				m_Weapon.transform.localEulerAngles = Vector3.zero;
				m_Weapon.postOutOfAmmo += ListenOutOfAmmo;
				m_Weapon.postCooldown += ListenWeaponCooldown;

				var _animationGroup = WeaponHelper.GetTrigger(m_Weapon.animationGroup);
				animator.SetTrigger(CharacterAnimationTrigger.ArmWeaponEquip(_animationGroup));
				animator.SetTrigger(CharacterAnimationTrigger.UpperWeaponEquip(_animationGroup));

				var _m_WeaponEquipPrf = Database.Weapon[m_Weapon.type].weaponEquipPrf;
				m_WeaponEquip = ((GameObject) Instantiate(_m_WeaponEquipPrf)).GetComponent<WeaponEquip>();
				m_WeaponEquip.transform.parent = weaponEquipPivot.transform;
				m_WeaponEquip.transform.localPosition = Vector3.zero;
				m_WeaponEquip.transform.localEulerAngles = Vector3.zero;
				
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
		}
	}

	public GameObject weaponPivot;
	public GameObject weaponEquipPivot;

    public void SetWeapon(WeaponType _weaponType)
    {
        var _weaponData = Database.Weapon[_weaponType];
        if (!_weaponData) return;

        var _weaponPrf = _weaponData.weaponPrf;
        var _weapon = (GameObject) Instantiate(_weaponPrf.gameObject);
        weapon = _weapon.GetComponent<Weapon>();
    }

	[RPC]
	private void Character_SetWeapon(NetworkViewID _viewID, int _weapon)
	{
		var _weaponType = WeaponHelper.Convert(_weapon);
		if (_weaponType == WeaponType.NONE) 
			return;

		var _weaponData = Database.Weapon[_weaponType];
		var _weaponObj = (GameObject) Instantiate(_weaponData.weaponPrf.gameObject);
		weapon = _weaponObj.GetComponent<Weapon>();
		weapon.networkView.enabled = true;
		weapon.networkView.viewID = _viewID;
	}

	private float m_Aim = 90;
	public float aim { 
		get { return m_Aim; }
		set { 
			if (isDead) return;
			if (Math.Abs(m_Aim - value) < 0.01) return;

			m_Aim = value;

		    if (m_ShouldAimWeapon)
		    {
                var _weaponAngle = weaponPivot.transform.eulerAngles;
                _weaponAngle.z = m_Aim - 90;
                weaponPivot.transform.eulerAngles = _weaponAngle;
                weaponEquipPivot.transform.eulerAngles = _weaponAngle;
                animator.SetFloat("aim", m_Aim);
		    }

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
	#endregion

	#region events
    public Action<Character> postDead;
    public Action<Character, Weapon> postWeaponChanged;
    public Action<Character, Crate> postObtainCrate;
	#endregion

	#region network
	public bool IsMine()
	{
		return ownerPlayer == Network.player.guid;
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

    #region debug
    public bool debugInvinsible = false;
    #endregion

    public Character()
    {
        isDead = false;
        floating = false;
    }

    void Awake () {
        id = s_Random.Next();

		m_Hp = GetComponent<PropertyHP>();
		m_Hp.hp = hpMax;
		m_Hp.postDead = Die;

        // life state
        {
            hitDisabled.postChanged += (_counter, _val) =>
            {
                if (_counter == 0)
                    damageDetector.enabled = true;
                else if (_counter.old == 0)
                    damageDetector.enabled = false;
            };
        }

		// components
		m_NetworkAnimator = GetComponent<NetworkAnimator>();
		m_InterpolatePosition = gameObject.AddComponent<InterpolatePosition>();

		m_AnimationEventor.postThrowAway += ListenAnimationEventThrowAway;

		// detector
		crateDetector.doObtain = Obtain;
		damageDetector.doDamage = Hit;
		
		terrainDetector.postDetect = (Collision) =>
		{
			floating = false;
			terrainDetector.gameObject.SetActive(false);
		};
	}
	
	void DestroySelf()
	{
		if (networkView.enabled)
			Network.Destroy(networkView.viewID);
		else
			Destroy(gameObject);
	}

	void Update() 
	{
		m_JumpCooltime -= Time.deltaTime;
		animator.SetFloat("speed_x", direction * rigidbody2D.velocity.x);
		animator.SetFloat("velocity_y", rigidbody2D.velocity.y);

		var _rayResult = Physics2D.Raycast(transform.position, -Vector2.up, 1f, terrainMask);

		if (_rayResult) {
			var _tilt = Mathf.Atan2(_rayResult.normal.y, direction * _rayResult.normal.x);
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
		get { return ! floating && m_JumpCooltime <= 0 && ! isDead; }
	}
	
	public void Jump()
	{
		floating = true;
		m_JumpCooltime = jumpCooldown;
		rigidbody2D.velocity += new Vector2(0, jumpVelocity);
        Invoke("EnableDetectTerrain", jumpCooldown);
		m_NetworkAnimator.SetTrigger(CharacterAnimationTrigger.JUMP_LOWER);
		m_NetworkAnimator.SetTrigger(CharacterAnimationTrigger.JUMP_UPPER);
	}

    void EnableDetectTerrain()
    {
        terrainDetector.gameObject.SetActive(true);
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

    private const int INTERNAL_HIT_FLAG = 21674;

	void EnableHit() {
        hitDisabled -= INTERNAL_HIT_FLAG;
	}

	public void Hit(AttackData _attackData) 
	{
//		Debug.Log("Hit");

		if (hitDisabled) return;

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
		hitDisabled += INTERNAL_HIT_FLAG;
		CancelInvoke("EnableHit");
		Invoke("EnableHit", hitCooldown);
		
		lastAttackData = _attackData;
		
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
		hitDisabled += INTERNAL_HIT_FLAG;
		
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

        SetWeapon(_crate.weapon);
	    if (postObtainCrate != null) postObtainCrate(this, _crate);
	}

	private void ListenOutOfAmmo(Weapon _weapon)
	{
        m_ShouldAimWeapon = false;
        // note: Unequip is called by animator.
        m_NetworkAnimator.SetTrigger("throw_away");
	}
	
	private void ListenWeaponCooldown(Weapon _weapon)
	{
		m_NetworkAnimator.SetTrigger ("rest");
	}

	public void Unequip()
	{
		weapon = null;
        if (m_WeaponEquip) ThrowAway();
	}

    public void ThrowAway()
    {
        if (!m_WeaponEquip)
        {
            Debug.LogWarning("Trying to throw away but weapon does not exist. Ignore.");
            return;
        }

        m_WeaponEquip.physicsEnabled = true;
        m_WeaponEquip.transform.parent = null;
        m_WeaponEquip.transform.localScale = Vector3.one;

        var _position = m_WeaponEquip.transform.localPosition;
        _position.x += -0.5f;
        m_WeaponEquip.transform.localPosition = _position;

        m_WeaponEquip.rigidbody2D.velocity += -3 * new Vector2 { x = transform.right.x, y = transform.right.y, };
        m_WeaponEquip.rigidbody2D.angularVelocity += transform.right.x > 0 ? 100 : -100;

        Destroy(m_WeaponEquip.gameObject, 2);
        m_WeaponEquip = null;
    }

	void OnSerializeNetworkView(BitStream _stream, NetworkMessageInfo _info) 
	{
		m_InterpolatePosition.OnSerializeNetworkView(_stream, _info);
		
		float _aim = 0;
		var _direction = 0;

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
