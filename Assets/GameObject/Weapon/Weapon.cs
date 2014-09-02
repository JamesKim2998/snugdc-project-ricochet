using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour 
{
	public WeaponType type;
	public WeaponAnimationGroup animationGroup;

	private GameObject m_Owner;
	public GameObject owner { get { return m_Owner; } set { m_Owner = value; }}

	public bool useDamage = true;
	public int damageExpose;
	public int? m_Damage;
	public int? damage { get { return m_Damage; } set { m_Damage = value; } }

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
	
			/*
			switch (value)
			{
			case State.SHOOTING:
				if (! IsState(State.PREPARING) || IsState(State.CHARGING) ) 
					return;
				break;
			}
			*/

			m_StateTime = 0;
			m_State = value; 

			switch (m_State) 
			{
			case State.SHOOTING:
				++m_ShootCount;
				++m_ShootIdx;
				ShootProc();
				break;
			case State.COOLING:
				m_ShootIdx = 0;
				Cool();
				break;
			}
		}
	} 

	// state
	public bool IsState(State _state) { return _state == m_State; }

	// trigger
	public bool autoload = false;
	public int shootAtOnce = 1;
	public int ammoMax = 1000;
	private int m_Ammo = 0;
	public int ammo { get { return m_Ammo; } }
	
	public bool IsNetworkEnabled() {
		return networkView.isMine && networkView.enabled && (Network.peerType != NetworkPeerType.Disconnected);
	}

	public void consumeAmmo() {
		if (m_Ammo <= 0) 
		{
			Debug.Log("Trying to consume not existing ammo! Ignore.");
			return;
		}

		m_Ammo -= 1;

		if (m_Ammo == 0)
		{
			if (postOutOfAmmo != null)
				postOutOfAmmo(this);
		}

	}

	public delegate void PostOutOfAmmo(Weapon _weapon);
	public PostOutOfAmmo postOutOfAmmo;

	// flag
	public bool relativeVelocityEnabled = true;

	public Vector3 projectileOffset;

	// time
	private float m_StateTime;
	public float stateTime { get { return m_StateTime; } }

	public float prepareTime;
	public float chargeTime;
	public float shootTime;
	public float cooldown;

	// shoot/projectile idx
	private int m_ShootCount = 0;
	public int shootCount { get { return m_ShootCount; } private set { m_ShootCount = value; } }
	private int m_ShootIdx = 0;
	public int shootIdx { get { return m_ShootIdx; } private set { m_ShootIdx = value; } }

	private int m_ProjectileCount = 0;
	public int projectileCount { get { return m_ProjectileCount; } }
	private int m_ProjectileIdx;
	public int projectileIdx { get { return m_ProjectileIdx; }}

	// projectile construct
	public delegate bool DoIsShootable(Weapon self);
	public DoIsShootable doIsShootable;
	
	public delegate int DoGetBundle(Weapon self);
	public DoGetBundle doGetBundle;

	public delegate GameObject DoCreateProjectile(Weapon self);
	public DoCreateProjectile doCreateProjectile;

	public delegate void DoShoot(Weapon self, GameObject projectile);
	public DoShoot doShoot;

	public delegate GameObject DoCreateProjectileServer(int _count, int _idx);
	public DoCreateProjectileServer doCreateProjectileServer;

	// events
	public delegate void PostShoot(Weapon _weapon, Projectile _projectile);
	public event PostShoot postShoot;
	
	public delegate void PostCooldown(Weapon _weapon);
	public event PostCooldown postCooldown;

	// effects
	public GameObject effectMuzzleFirePrf;
	public Vector3 effectMuzzleFireOffset;

	public void Start()
	{
		m_Ammo = ammoMax;
		if (useDamage) damage = damageExpose;
	}

	public void Update () 
	{
		if (networkView.enabled && ! networkView.isMine)
			return;

		m_StateTime += Time.deltaTime;

		switch (m_State) 
		{
		case State.IDLE: break;
		case State.SHOOTING: {
			if (m_StateTime > shootTime)
			{
				if (autoload || (shootIdx < shootAtOnce) )
				{
					state = State.CHARGING;
				}
				else 
				{
					state = State.COOLING;
				}
			}
			break;
		}

		case State.CHARGING: {
			if (m_StateTime > chargeTime)
			{
				state = State.SHOOTING;
			}
			break;
		}

		case State.COOLING: {
			if (m_StateTime > cooldown) {
				state = State.IDLE;
			}
			break;
		}
		}
	}

	public bool IsShootable() {
		return m_Ammo > 0 && (doIsShootable == null || ! doIsShootable(this));
	}

	public void Shoot() {
		state = State.SHOOTING;
	}

	private void ShootProc() 
	{
		if (! IsShootable()) {
			Debug.LogError("trying to shoot not shootable weapon!");
		}

		int _bundle = 1;

		if (doGetBundle != null) {
		 	_bundle = doGetBundle(this);
		}

		for (m_ProjectileIdx = 0; m_ProjectileIdx < _bundle && ammo > 0; ++m_ProjectileIdx) {

			var _projectileGO = doCreateProjectile(this);

			++m_ProjectileCount;

			consumeAmmo();

			if (owner) {
				_projectileGO.transform.rotation *= transform.rotation;

				_projectileGO.transform.position += transform.position;
				_projectileGO.transform.Translate( projectileOffset);

				if (relativeVelocityEnabled) {
					_projectileGO.rigidbody2D.velocity 
						+= owner.rigidbody2D.velocity;
				}
			}

			var _projectile = _projectileGO.GetComponent<Projectile>();
			
			if (owner != null)
			{
				_projectile.ownerID = owner.GetInstanceID();
				_projectile.ownerDeadZoneCollider = owner.GetComponent<Character>().deadZoneCollider;
				var _detector = owner.GetComponentInChildren<DamageDetector>();
				if (_detector) _projectile.ownerDetecterID = _detector.GetInstanceID();
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
				_projectile.attackData.owner = Network.player.guid;

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
		{
			networkView.RPC("PlayMuzzleFireEffect", RPCMode.All);
		}
		else
		{
			PlayMuzzleFireEffect();
		}
		
		if (ammo <= 0)
		{
			Rest();
		}
	}

	[RPC]
	void Weapon_RequestCreateProjectileServer(NetworkViewID _viewID, string _owner, Vector3 _position, Quaternion _rotation, Vector3 _velocity, int _count, int _idx)
	{
		var _projectileGO = doCreateProjectileServer(_count, _idx);

		_projectileGO.transform.position = _position;
		_projectileGO.transform.rotation = _rotation;
		_projectileGO.rigidbody2D.velocity = _velocity;

		if (owner != null)
		{
			var _projectile = _projectileGO.GetComponent<Projectile>();
			_projectile.ownerID = owner.GetInstanceID();
			_projectile.attackData.owner = _owner;
			var _detector = owner.GetComponentInChildren<DamageDetector>();
			if (_detector) _projectile.ownerDetecterID = _detector.GetInstanceID();
		}

		_projectileGO.networkView.viewID = _viewID;
		_projectileGO.networkView.enabled = true;
	}

	[RPC]
	void PlayMuzzleFireEffect()
	{
		if(effectMuzzleFirePrf != null)
		{
			var _effect = GameObject.Instantiate (effectMuzzleFirePrf, transform.position, transform.rotation) as GameObject;
			_effect.transform.Translate (effectMuzzleFireOffset);
		}
	}

	public void Stop() {
		
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
