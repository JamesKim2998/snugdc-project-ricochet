using UnityEngine;	
using System;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour {

	private bool m_Activated = false;
	public bool activated {
		get { return m_Activated; }
		set { 
			if (m_Activated == value) return;
			m_Activated = value; 
		}
	}

	public int ownerID = 0;
	public int ownerDetecterID = 0;

	// life
	public float life = 10;
	private float m_Age = 0;

	// attack
	public bool isHitOwner = false;
	public AttackData attackData;
	public int damage { set { attackData.damage = value; }}

	// physics
	public Vector2 initialVelocity = Vector2.zero;
	public Vector2 drivingForce = Vector2.zero;
	public Vector2 relativeDrivingForce = Vector2.zero;

	// filter
	public LayerMask collisionIgnores;
	public LayerMask collisionTargets;
	public LayerMask collisionTerrains;

	// prepare
	public float prepareDuration = 0;
	private float m_PrepareTime;

	// decay
	private bool m_Decaying = false;
	public bool decaying { get { return m_Decaying; } }
	public bool stopOnDecay = true;
	public float decayDuration = 0;
	private float m_DecayTime;

	// components
	private Ricochet m_Ricochet;
	private Animator m_Animator;

	// collide with something
	public delegate void PostCollide(Projectile _projectile, Collider2D _collider);
	public event PostCollide postCollide;

	// collide with targets
	public delegate void PostHit(Projectile _projectile, Collider2D _collider);
	public event PostHit postHit;

	// collide with terrains
	public delegate void PostBumped(Projectile _projectile, Collider2D _collider);
	public event PostBumped postBumped;


	void Start () 
	{
		if (initialVelocity != Vector2.zero)
			rigidbody2D.velocity = initialVelocity;

		m_Ricochet = GetComponent<Ricochet>();
		m_Animator = GetComponent<Animator>();

		if (! prepareDuration.Equals(0)) 
		{
			m_PrepareTime = 0;
		} 
		else 
		{
			activated = true;
		}
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
	
	void StartDecay() {
		if (decaying) return;
		
		m_Decaying = true;
		
		if (decayDuration.Equals(0)) 
		{
			DestroySelf();
			return;
		}
		
		m_DecayTime = 0;
		
		if (stopOnDecay) 
		{
			rigidbody2D.velocity = new Vector2(0, 0);
			rigidbody2D.isKinematic = true;
		}
	}

	void Update () 
	{
		var dt = Time.deltaTime;

		if (! activated) 
		{
			if (m_PrepareTime < prepareDuration) 
			{
				m_PrepareTime += dt;
				
				if (m_PrepareTime >= prepareDuration) 
				{
					activated = true;

					if (m_Animator != null) 
						m_Animator.SetTrigger("Activate");
				}
			} 
		}

		if (activated) {

			if ( m_Age < life) 
			{
				m_Age += dt;

				if (m_Age >= life )
					StartDecay();
			}
		} 

		if (m_Decaying) 
		{
			m_DecayTime += dt;
			if (m_DecayTime > decayDuration) 
				DestroySelf();
		}
	}

	void FixedUpdate() 
	{
		if (! m_Decaying) 
		{
			if (! rigidbody2D.isKinematic)
			{
				Vector2 _drivingForce = Vector2.zero;
				
				if (! drivingForce.Equals(Vector2.zero)) 
					_drivingForce += drivingForce;
				
				if (! relativeDrivingForce.Equals(Vector2.zero)) 
				{
					// todo: opt
					var _drivingForceWorld = transform.localToWorldMatrix.MultiplyVector(relativeDrivingForce);
					_drivingForce += new Vector2(_drivingForceWorld.x, _drivingForceWorld.y);
				}
				
				if (_drivingForce != Vector2.zero)
					rigidbody2D.AddForce(_drivingForce);
			}
		}
	}

	void OnCollisionEnter2D(Collision2D _collision)
	{
		OnCollision(_collision.collider);
	}

	void OnTriggerEnter2D (Collider2D _collider)
	{
		OnCollision(_collider);
	}

	void OnCollision(Collider2D _collider) 
	{
		if (! activated) return;
		if (decaying) return;

		if (LayerHelper.Exist(collisionIgnores, _collider)) return;

		if (m_Ricochet && ! m_Ricochet.ShouldCollide(_collider)) return;

		if (m_Ricochet)
		{
			if (! m_Ricochet.OnCollision(_collider))
				StartDecay();
		}
		else 
		{
			StartDecay();
		}

		if (LayerHelper.Exist(collisionTargets, _collider)) 
		{
			var _damageDetector = _collider.GetComponentInChildren<DamageDetector>();

			if (! isHitOwner 
			    && (_damageDetector.GetInstanceID() == ownerDetecterID))
				return;

			if (_damageDetector != null
			    && _damageDetector.enabled) 
			{
				attackData.velocity = rigidbody2D.velocity;
				_damageDetector.Damage(attackData);
			}
			
			if (postHit != null) 
				postHit(this, _collider);
		}
		else if (LayerHelper.Exist(collisionTargets, _collider.gameObject))
		{
			if (postBumped != null) 
				postBumped(this, _collider);
		}
		
		if (postCollide != null) 
			postCollide(this, _collider);

	}


}
