using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWeapon : Weapon {

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
				Shoot();
				break;
			case State.COOLING:
				m_ShootIdx = 0;
				Cool();
				break;
			}
		}
	} 

	public class ShootArgs : EventArgs {
		public GameObject projectile;
	}

	// state
	// todo: incomplete
	public bool IsState(State _state) { return _state == m_State; }

	public override bool isShooting { get { return IsState(State.SHOOTING); }}
	public override bool isCooling { get { return IsState(State.COOLING); }}

	// trigger
	public bool autoload = false;
	public int shootAtOnce = 1;
	public int ammoMax = 1000;
	private int m_Ammo = 0;
	public int ammo { get { return m_Ammo; } }

	// flag
	public bool relativeVelocityEnabled = true;

	// time
	private float m_StateTime;
	public float stateTime { get { return m_StateTime; } }

	public float prepareTime;
	public float chargeTime;

	private float m_ShootTime;
	public float shootTime { get { return m_ShootTime; } set { m_ShootTime = value; }}

	private float m_Cooldown;
	public override float cooldown { get { return m_Cooldown; } set { m_Cooldown = value; }}

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
	public delegate void PostShoot(object sender, ShootArgs args);
	public event PostShoot postShoot;
	
	public delegate void PostCooldown(object sender, EventArgs args);
	public event PostCooldown postCooldown;

	public void Start()
	{
		m_Ammo = ammoMax;
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
				if (autoload || (shootIdx >= shootAtOnce) )
				{
					state = State.COOLING;
				}
				else 
				{
					state = State.CHARGING;
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

	public override bool IsShootable() {
		if (doIsShootable != null && ! doIsShootable(this)) return false;
		return true;
	}

	public override void Shoot() 
	{
		if (! IsShootable()) {
			Debug.LogError("trying to shoot not shootable weapon!");
		}

		int _bundle = 1;

		if (doGetBundle != null) {
		 	_bundle = doGetBundle(this);
		}

		for (m_ProjectileIdx = 0; m_ProjectileIdx < _bundle; ++m_ProjectileIdx) {

			var _projectile = doCreateProjectile(this);

			++m_ProjectileCount;

			if (owner) {
				// todo: test
				var projectilePosition = _projectile.transform.position;

				_projectile.transform.position = projectilePosition
					+ transform.position;

				if (relativeVelocityEnabled) {
					_projectile.rigidbody2D.velocity 
						+= owner.rigidbody2D.velocity;
				}
			}

			var _theProjectile = _projectile.GetComponent<Projectile>();
			
			if (owner != null)
			{
				_theProjectile.ownerID = owner.GetInstanceID();
				var _detector = owner.GetComponentInChildren<DamageDetector>();
				if (_detector) _theProjectile.ownerDetecterID = _detector.GetInstanceID();
			}

			if (_theProjectile) {

				if (damage.HasValue) {
					_theProjectile.damage = damage.Value;
				}
			}

			if (doShoot != null) {
				doShoot(this, _projectile);
			}

			if (networkView.enabled)
			{
				_projectile.networkView.viewID = Network.AllocateViewID();
				_projectile.networkView.enabled = true;

				networkView.RPC("CreateProjectileServer", 
				                RPCMode.Others, 
				                _projectile.networkView.viewID, 
				                _projectile.transform.position, 
				                _projectile.transform.localRotation, 
				                (Vector3) _projectile.rigidbody2D.velocity, 
				                projectileCount, 
				                projectileIdx);
			}

			if (postShoot != null) {
				var args = new ShootArgs();
				args.projectile = _projectile;
				postShoot(this, args);
			}
		}

	}

	[RPC]
	void CreateProjectileServer(NetworkViewID _viewID, Vector3 _position, Quaternion _rotation, Vector3 _velocity, int _count, int _idx)
	{
		var _projectile = doCreateProjectileServer(_count, _idx);

		_projectile.transform.position = _position;
		_projectile.transform.rotation = _rotation;
		_projectile.rigidbody2D.velocity = _velocity;

		if (owner != null)
		{
			var _theProjecttile = _projectile.GetComponent<Projectile>();
			_theProjecttile.ownerID = owner.GetInstanceID();
			var _detector = owner.GetComponentInChildren<DamageDetector>();
			if (_detector) _theProjecttile.ownerDetecterID = _detector.GetInstanceID();
		}

		_projectile.networkView.viewID = _viewID;
		_projectile.networkView.enabled = true;
	}

	public override void Stop() {
		
		if (isShooting) {
			Cool();
		}

		state = State.IDLE;
	}

	private void Cool() 
	{
		if (postCooldown != null) postCooldown(this, null); 
	}

	public void Rest() 
	{
		state = State.IDLE;
	}
}
