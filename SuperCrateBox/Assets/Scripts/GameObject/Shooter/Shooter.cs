using UnityEngine;

public class Shooter : MonoBehaviour
{
    private bool m_Floating = false;
	public bool floating { get {return m_Floating; }}

    private float m_JumpCooltime = 0.0f;

    public float jumpForce = 10.0f;
    public float jumpCooldown = 1.0f;
    public float moveForce = 10.0f;
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

	// hp
	public int hpMax = 1;
	HasHP m_Hp;

	private int m_Direction = 1;

	public int direction {
		get { return m_Direction; }
		set {
			if (m_Direction == value) return;
			
			m_Direction = value;

			var _scale = transform.localScale;
			_scale.x = value * Mathf.Abs(_scale.x);
			transform.localScale = _scale;
			
			if (weapon) {
				weapon.direction = new Vector2(value, 0);
			}
		}
	}

	// weapon
	private Weapon m_Weapon;

	public Weapon weapon { 
		get { return m_Weapon; } 
		set { 
			if (m_Weapon != null) 
				Destroy(m_Weapon.gameObject);

			m_Weapon = value;

			if (m_Weapon != null) {
				m_Weapon.direction = new Vector2(direction, 0);
				m_Weapon.transform.position += transform.position;
				m_Weapon.transform.position += new Vector3(direction * weaponPosition.x, weaponPosition.y);
				m_Weapon.transform.rotation = transform.rotation;
				m_Weapon.transform.parent = transform;
				
				m_Weapon.owner = gameObject;
			}

		}
	}

	public Vector2 weaponPosition;

	// components
	Animator m_Anim;

	// detector
	public CrateDetector crateDetector;
	public DamageDetector damageDetector;
    public TerrainDetector terrainDetector;
	
	// events
	public delegate void PostDead(Shooter _shooter);
	public event PostDead postDead;

    void Start()
    {
		// hp
		m_Hp = GetComponent<HasHP>();
		m_Hp.hp = hpMax;
		m_Hp.postDead = Die;

		// weapon
		weapon = null;
//		var _weapon = GameObject.Instantiate(Resources.Load("rocket_luncher")) as GameObject;
//		weapon = _weapon.GetComponent<Weapon>();

		// components
		m_Anim = GetComponent<Animator>();

		// detector
		crateDetector.doObtain = Obtain;
		damageDetector.doDamage = Hit;

        terrainDetector.postDetect = (Collision) =>
        {
            m_Floating = false;
			terrainDetector.gameObject.SetActive(false);
        };
    }

    void Update()
    {
		m_JumpCooltime -= Time.deltaTime;

    }

	public bool movable { 
		get { return ! isDead; }
	}

    public void Move(float _direction)
    {
		if (Mathf.Abs(_direction) > 0.01f) {
			direction = (int) Mathf.Sign(_direction);
		}

		rigidbody2D.AddForce(new Vector2(_direction * moveForce, 0));
    }

	public bool jumpable {
		get { return ! m_Floating && m_JumpCooltime <= 0; }
	}

	public void Jump()
    {
        m_Floating = true;
        m_JumpCooltime = jumpCooldown;
        rigidbody2D.velocity += new Vector2(0, jumpForce);
		terrainDetector.gameObject.SetActive(true);
    }

	public bool shootable {
		get {
			return weapon != null && weapon.IsShootable(); 
		}
	}

	public void Shoot() {
		weapon.Shoot();
	}

	void EnableHit() {
		hitEnabled = true;
	}

	void Hit(AttackData _attackData) {
		if (! hitEnabled) return;
		hitEnabled = false;

		Invoke("EnableHit", hitCooldown);

		var _direction = Mathf.Sign(_attackData.velocity.x);
		rigidbody2D.AddForce(new Vector2(_direction * hitForce.x, hitForce.y));

		m_Hp.damage(_attackData);

		if (m_Hp > 0) {
			m_Anim.SetTrigger("Hit");
		}
	}

	void DestroySelf() {
		Destroy(gameObject);
	}

	void Die() {
		CancelInvoke("EnableHit");

		isDead = true;
		hitEnabled = false;

		var _deadForce = deadForce;
		_deadForce.x *= -direction;

		rigidbody2D.velocity = new Vector2(0, 0);
		rigidbody2D.AddForce(_deadForce);

		m_Anim.SetTrigger("Dead");

		Game.Statistic().death.val += 1;
		if (this.postDead != null) this.postDead(this);

		Invoke("DestroySelf", deadDelay);
	}
	
	void Obtain(Crate _crate) {
		
		if (_crate.empty) return;

		var _weapon = GameObject.Instantiate(_crate.weapon) as GameObject;
		weapon = _weapon.GetComponent<Weapon>();
	}
}