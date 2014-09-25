using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Weapon : MonoBehaviour 
{
	public WeaponType type;
	public WeaponAnimationGroup animationGroup;

    public string ownerPlayer { get; set; }
    public GameObject ownerGameObj { get; set; }

    #region editor properties
    public bool useDamage = true;
	public int editorDamage = 1;
    public bool useShootBundle = false;
    public int editorShootBundle = 1;
    #endregion

	#region state
	public enum State {
		IDLE,
		PREPARING,
		CHARGING,
		SHOOTING,
		COOLING,
	}

	private State m_State = State.IDLE;
	public State state { 
		get { return m_State; } 
		private set { 
			if (m_State == value) 
			{
				Debug.Log("Trying to set same state again. Ignore.");
			}
	
			stateTime = 0;
			m_State = value; 

			switch (m_State) 
			{
			case State.SHOOTING:
				++shootCount;
				++shootIdx;
				ShootProc();
				break;
			case State.COOLING:
				shootIdx = 0;
				Cool();
				break;
			}
		}
    }

    public bool IsState(State _state) { return _state == m_State; }

    // time
    public float stateTime { get; private set; }

    public float prepareTime;
    public float chargeTime;
    public float shootTime;
    public float cooldown;

	#endregion

	#region shoot option
	// shoot option
    public int shootBundle
    {
        get
        {
            if (doGetBundle == null)
                return 1;
            else
                return doGetBundle(this);
        }

        set
        {
            if (doGetBundle != null)
                Debug.LogWarning("doGetBundle already exists. Replace.");
            doGetBundle = delegate { return value; };
        }
    }

    public bool autoload = false;
	public int shootAtOnce = 1;
	#endregion

    #region ammo
    public int ammoMax = 1;
    public int ammo { get; private set; }

	public void consumeAmmo() {
		if (ammo <= 0) 
		{
			Debug.Log("Trying to consume not existing ammo! Ignore.");
			return;
		}

		--ammo;

		if (ammo == 0)
		{
			if (postOutOfAmmo != null)
				postOutOfAmmo(this);
		}

	}

	public Action<Weapon> postOutOfAmmo;
    #endregion

	#region projectile properties
    public int? damage { get; set; }
    public bool relativeVelocityEnabled = true;

    public Vector3 projectileOffset;

    #endregion

	#region projectile construction
	// shoot/projectile idx
    public int shootCount { get; private set; }
    public int shootIdx { get; private set; }

    public int projectileCount { get; private set; }
    public int projectileIdx { get; private set; }

    // projectile construct
	public delegate bool DoIsShootable(Weapon self);
	public DoIsShootable doIsShootable;
	
	public delegate int DoGetBundle(Weapon self);
	public DoGetBundle doGetBundle;

	public delegate GameObject DoCreateProjectile(Weapon self);
	public DoCreateProjectile doCreateProjectile;

	public Action<Weapon, GameObject> doShoot;

	public delegate GameObject DoCreateProjectileServer(int _count, int _idx);
	public DoCreateProjectileServer doCreateProjectileServer;
	#endregion

	#region event
	// events
	public delegate void PostShoot(Weapon _weapon, Projectile _projectile);
	public event PostShoot postShoot;
	
	public delegate void PostCooldown(Weapon _weapon);
	public event PostCooldown postCooldown;
	#endregion

	#region effect
	// effects
	public GameObject effectMuzzleFirePrf;
	public Vector3 effectMuzzleFireOffset;
	#endregion

    #region network
    public bool IsNetworkEnabled()
    {
        return networkView.isMine && networkView.enabled && (Network.peerType != NetworkPeerType.Disconnected);
    }
    #endregion

    #region misc properties

    public float recoil = 0;
    public Vector2 recoilModifier = Vector2.one;

    #endregion

    public Weapon()
    {
        ammo = 0;
        shootIdx = 0;
        shootCount = 0;
        projectileCount = 0;
    }

    public void Awake()
    {
        ammo = ammoMax;
        if (useDamage) damage = editorDamage;
        if (useShootBundle) shootBundle = editorShootBundle;
    }

	public void Update () 
	{
		if (networkView.enabled && ! networkView.isMine)
			return;

		stateTime += Time.deltaTime;

		switch (m_State) 
		{
		case State.IDLE: break;
		case State.SHOOTING: {
			if (stateTime > shootTime)
			{
				if (autoload || (shootIdx < shootAtOnce) )
					state = State.CHARGING;
				else 
					state = State.COOLING;
			}
			break;
		}

		case State.CHARGING: {
			if (stateTime > chargeTime)
				state = State.SHOOTING;
			break;
		}

		case State.COOLING: {
			if (stateTime > cooldown)
				state = State.IDLE;
			break;
		}
		}
	}

	public bool IsShootable() {
		return ammo > 0 
            && IsState(State.IDLE)
            && (doIsShootable == null || ! doIsShootable(this));
	}

	public void Shoot() {
		state = State.SHOOTING;
	}

	private void ShootProc() 
	{
	    var _bundle = shootBundle;

	    Vector2? _recoil = null;
	    if (! recoil.Equals(0) && ownerGameObj && ownerGameObj.rigidbody2D)
	    {
	        var _tmp = recoil*-transform.right;
	        _tmp.x *= recoilModifier.x;
            _tmp.y *= recoilModifier.y;
	        _recoil = _tmp;
	    }

	    for (projectileIdx = 0; projectileIdx < _bundle && ammo > 0; ++projectileIdx) 
        {
			var _projectileGO = doCreateProjectile(this);

			++projectileCount;

			consumeAmmo();

			if (ownerGameObj) 
            {
				_projectileGO.transform.rotation *= transform.rotation;

				_projectileGO.transform.position += transform.position;
				_projectileGO.transform.Translate( projectileOffset);

				if (relativeVelocityEnabled) 
                {
					_projectileGO.rigidbody2D.velocity 
						+= ownerGameObj.rigidbody2D.velocity;
				}
			}

			var _projectile = _projectileGO.GetComponent<Projectile>();
            _projectile.ownerPlayer = ownerPlayer;
            _projectile.ownerWeapon = type;
            
			if (ownerGameObj != null)
			{
				_projectile.ownerID = ownerGameObj.GetInstanceID();

                var _deadzone = _projectile.GetComponent<ProjectileDecoratorDeadzone>();
			    var _deadzoneField = ownerGameObj.GetComponent<DeadzoneField>();
			    if (_deadzone && _deadzoneField)
			        _deadzone.deadzone = _deadzoneField.deadzone;

                if (_recoil.HasValue)
                    ownerGameObj.rigidbody2D.velocity += _recoil.Value;

				var _detector = ownerGameObj.GetComponentInChildren<DamageDetector>();
                if (_detector) _projectile.ownerDamageDetecterID = _detector.GetInstanceID();
			}

			if (_projectile) 
			{
				if (damage.HasValue) 
					_projectile.damage = damage.Value;
			}

			if (doShoot != null) 
				doShoot(this, _projectileGO);

            if (IsNetworkEnabled())
			{
				_projectileGO.networkView.viewID = Network.AllocateViewID();
				_projectileGO.networkView.enabled = true;

				networkView.RPC("Weapon_RequestCreateProjectileServer", 
				                RPCMode.Others, 
				                _projectile.networkView.viewID, 
				                Network.player.guid,
				                _projectileGO.transform.position, 
				                _projectileGO.transform.localRotation, 
				                (Vector3) _projectileGO.rigidbody2D.velocity, 
				                projectileCount, 
				                projectileIdx);
			}

			if (postShoot != null) 
				postShoot(this, _projectileGO.GetComponent<Projectile>());
		}

		if (IsNetworkEnabled())
			networkView.RPC("PlayMuzzleFireEffect", RPCMode.All);
		else
			PlayMuzzleFireEffect();
		
		if (ammo <= 0)
			Rest();
	}

	[RPC]
	void Weapon_RequestCreateProjectileServer(NetworkViewID _viewID, string _ownerPlayer, Vector3 _position, Quaternion _rotation, Vector3 _velocity, int _count, int _idx)
	{
		var _projectileGO = doCreateProjectileServer(_count, _idx);

		_projectileGO.transform.position = _position;
		_projectileGO.transform.rotation = _rotation;
		_projectileGO.rigidbody2D.velocity = _velocity;

        var _projectile = _projectileGO.GetComponent<Projectile>();
        _projectile.ownerPlayer = _ownerPlayer;
        _projectile.ownerWeapon = type;

		if (ownerGameObj != null)
		{
			_projectile.ownerID = ownerGameObj.GetInstanceID();
			var _detector = ownerGameObj.GetComponentInChildren<DamageDetector>();
			if (_detector) _projectile.ownerDamageDetecterID = _detector.GetInstanceID();
		}

        _projectileGO.networkView.viewID = _viewID;
        _projectileGO.networkView.enabled = true;
	}

	[RPC]
	void PlayMuzzleFireEffect()
	{
		if(effectMuzzleFirePrf != null)
		{
			var _effect = (GameObject) Instantiate (effectMuzzleFirePrf);
            TransformHelper.SetParentLocal(_effect, gameObject);
			_effect.transform.localPosition = effectMuzzleFireOffset;
		}
	}

	public void Stop() 
    {
		if (IsState(State.SHOOTING)) {
			Cool();
		}

		state = State.IDLE;
	}

	private void Cool() 
	{
		if (postCooldown != null) postCooldown(this); 
	}

	public void Rest() 
	{
		if (! autoload)
			return;

		switch (state)
		{
		case State.SHOOTING:
		case State.CHARGING:
			state = State.COOLING;
			break;
		case State.PREPARING:
			state = State.IDLE;
			break;
		}
	}
}
