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
	
	public Collider2D ownerDeadZoneCollider {
		set { 
			if ( value != null)
			{
				m_OwnerDeadZoneColliderID = value.GetInstanceID();
			}
			else
			{
				m_OwnerDeadZoneColliderID = 0;
			}
		}
	}

	private int m_OwnerDeadZoneColliderID = 0;

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

	// effect
	public GameObject effectHitPrf;
	public Vector3 effectHitOffset;
	
	// Dead-zone related variables.
	// 총알은 최초에 데드존 내에서 발사되어서, 데드존을 벗어나야만 플레이어에 대한 충돌판정이 활성화된다.
	private bool m_OutOfDeadZone;
	
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
		
		if (m_OwnerDeadZoneColliderID == 0)
		{
			Debug.Log ("Character doesn't have an detector");
			m_OutOfDeadZone = true;
		} 
		else 
		{
			if(IsProjectileInsideOfInitialDeadzone())
			{
				m_OutOfDeadZone = false;
			} 
			else 
			{
				m_OutOfDeadZone = true;
			}
		}
	}
	
	private bool IsProjectileInsideOfInitialDeadzone()
	{
		// I don't know how to implements.
		// 현재 생성된 Projectile이 deadzoneDetector 내부에 있는지 검사를했으면 좋겠는데
		// 마땅한 API가 없는듯.
		return true;
	}

	void DestroySelf() 
	{
		if (networkView.enabled && networkView.viewID != NetworkViewID.unassigned)
			Network.Destroy(networkView.viewID);
		else 
			GameObject.Destroy(gameObject);
	}
	
	void StartDecay() 
	{
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
	
	void OnTriggerExit2D(Collider2D other)
	{
		if(other.isTrigger )
		{
			if ( other.GetInstanceID() == m_OwnerDeadZoneColliderID)
			{
				Debug.Log ("Projectile just leaved owner's deadzone");
				m_OutOfDeadZone = true;
			}
		}
	}

	void OnCollision(Collider2D _collider) 
	{
		// todo: trigger라고 맞지 않는다는 보장이 없음. 위험함. 
		if (_collider.isTrigger) return;
		if (! activated) return;
		if (decaying) return;

		if (LayerHelper.Exist(collisionIgnores, _collider)) return;

		if (m_Ricochet)
		{
			if (! m_Ricochet.ShouldCollide(_collider))
				return;

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
			var _isOwner = _damageDetector.GetInstanceID() == ownerDetecterID;

			if (! isHitOwner && _isOwner)
				return;

			if ( isHitOwner && _isOwner && ! m_OutOfDeadZone)
			{
				Debug.Log("Projectile just hit owner before get out of the dead zone.");
				// Projectile is ignore the owner as if he wasn't there.
				return;
			}
				
			if (_damageDetector != null && _damageDetector.enabled) 
			{
				attackData.velocity = rigidbody2D.velocity;
				_damageDetector.Damage(attackData);
			}
			
			if (postHit != null) 
				postHit(this, _collider);

			// todo: server를 통해서 이루어져야합니다.
			if (effectHitPrf)
			{
				var _effectHit = GameObject.Instantiate (effectHitPrf, transform.position, transform.rotation) as GameObject;
				_effectHit.transform.Translate(effectHitOffset);
			}
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
