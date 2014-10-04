using UnityEngine;	
using System;
using System.Collections;
using System.Collections.Generic;

public class Projectile : MonoBehaviour
{
    public ProjectileType editorType = ProjectileType.NONE;

    public ProjectileType type
    {
        get { return attackData.projectile; }
        set { attackData.projectile = value; }
    }

    public string ownerPlayer
    {
        get { return attackData.ownerPlayer; }
        set { attackData.ownerPlayer = value; }
    }

    public WeaponType ownerWeapon
    {
        get { return attackData.weapon;  }
        set { attackData.weapon = value; }
    }
    
    [HideInInspector]
	public int ownerID = 0;

	[HideInInspector]
	public int ownerDamageDetecterID = 0;

    #region active

    private int m_ActivateCounter = 0;
    public bool activated { get { return m_ActivateCounter >= 0;  } }

    public bool Activate()
    {
#if DEBUG
        if (m_ActivateCounter == 0)
            Debug.LogWarning("Activate counter should not be greater than 0. ");
#endif
        return ++m_ActivateCounter == 0;
    }

    public bool Deactivate() { return m_ActivateCounter-- == 0; }

    #endregion

	#region life cycle
	// life
	public float life = 10;
	private float m_Age = 0;

    // prepare
    public float prepareDuration = 0;
    private float m_PrepareTime;

    // decay
    public bool decaying { get; private set; }
    public bool stopOnDecay = true;
    public float decayDuration = 0;
    private float m_DecayTime;
	#endregion

	// attack
	public bool isHitOwner = false;
    [HideInInspector]
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
	
    public Projectile()
    {
        decaying = false;
    }

    void Start ()
    {
        type = editorType;

		if (initialVelocity != Vector2.zero)
			rigidbody2D.velocity = initialVelocity;

		m_Ricochet = GetComponent<Ricochet>();
		m_Animator = GetComponent<Animator>();

		if (! prepareDuration.Equals(0)) 
        {
			m_PrepareTime = 0;
            Deactivate();
        }
	}
	
	void DestroySelf() 
	{
		if (networkView.enabled && networkView.viewID != NetworkViewID.unassigned)
			Network.Destroy(networkView.viewID);
		else 
			Destroy(gameObject);
	}
	
	void StartDecay() 
	{
		if (decaying) return;
		
		decaying = true;
		
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
					Activate();
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

		if (decaying) 
		{
			m_DecayTime += dt;
			if (m_DecayTime > decayDuration) 
				DestroySelf();
		}
	}

	void FixedUpdate() 
	{
		if (! decaying) 
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
		// todo: trigger라고 맞지 않는다는 보장이 없음. 위험함. 
		if (_collider.isTrigger) return;
		if (decaying) return;

		if (LayerHelper.Exist(collisionIgnores, _collider)) return;

		if (m_Ricochet)
		{
			if (! m_Ricochet.ShouldCollide(_collider))
				return;

			if (m_Ricochet.OnCollision(_collider))
				StartDecay();
		}
		else 
		{
			StartDecay();
		}

		if (LayerHelper.Exist(collisionTargets, _collider))
		{
		    if (! activated) 
                goto finalize;

            var _damageDetector = _collider.GetComponentInChildren<DamageDetector>();
			var _isOwner = _damageDetector.GetInstanceID() == ownerDamageDetecterID;

			if (! isHitOwner && _isOwner)
				return;
				
			if (_damageDetector && _damageDetector.enabled) 
			{
				attackData.velocity = rigidbody2D.velocity;
				_damageDetector.Damage(attackData);
			}
			
			if (postHit != null) 
				postHit(this, _collider);

			// todo: server를 통해서 이루어져야합니다.
			if (effectHitPrf)
			{
				var _effectHit = (GameObject) Instantiate (effectHitPrf, transform.position, transform.rotation);
				_effectHit.transform.Translate(effectHitOffset);
			}
		}
		else if (LayerHelper.Exist(collisionTargets, _collider.gameObject))
		{
			if (postBumped != null) 
				postBumped(this, _collider);
		}
		
	    finalize:

		if (postCollide != null) 
			postCollide(this, _collider);

	}


}
