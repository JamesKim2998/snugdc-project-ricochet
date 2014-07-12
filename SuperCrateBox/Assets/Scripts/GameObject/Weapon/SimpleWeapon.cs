using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleWeapon : Weapon {

	public class ShootArgs : EventArgs {
		public GameObject projectile;
	}

	// state
	private bool m_IsShooting = false;
	public override bool isShooting { get { return m_IsShooting; }}

	private bool m_IsCooling = false;
	public override bool isCooling { get { return m_IsCooling; }}

	// flag
	public bool relativeVelocityEnabled = true;

	// time
	private float m_ShootTime;
	public float shootTime { get { return m_ShootTime; } set { m_ShootTime = value; }}
	
	private float m_ShootTimeLeft;
	public float shootTimeLeft { get { return m_ShootTimeLeft; }}

	private float m_Cooldown;
	public override float cooldown { get { return m_Cooldown; } set { m_Cooldown = value; }}

	private float m_Cooltime;
	public float cooltime { get { return m_Cooltime; }}

	// projectile idx
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

	public void Update () {

		if (networkView.enabled && ! networkView.isMine)
			return;

		if (m_IsShooting) {

			m_ShootTimeLeft -= Time.deltaTime;

			if (m_ShootTimeLeft <= 0) {
				Cool();
			}
			
		} 

		if (m_IsCooling) {
			m_Cooltime -= Time.deltaTime;
			if (m_Cooltime <= 0) {
				m_IsCooling = false;
				if (postCooldown != null) postCooldown(this, null); 
			}
		}

	}

	public override bool IsShootable() {
		if (m_IsShooting || m_IsCooling ) return false;
		if (doIsShootable != null && ! doIsShootable(this)) return false;
		return true;
	}

	public override void Shoot() {

		if (! IsShootable()) {
			Debug.LogError("trying to shoot not shootable weapon!");
		}

		m_IsShooting = true;
		m_ShootTimeLeft = 0;

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
		} else {
			m_IsShooting = false;
			m_IsCooling = false;
		}

	}

	private void Cool() {

		if (! isShooting || isCooling) return;

		m_IsShooting = false;
		m_IsCooling = true;
		m_Cooltime = cooldown;
	}
}
