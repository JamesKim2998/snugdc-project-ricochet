using UnityEngine;
using System.Collections;
using System;

public class Enemy : MonoBehaviour {

	public int score = 1;

	// components
	protected HasHP m_HP;

	private Agent m_Agent;
	public Agent agent { get { return m_Agent; } }

	protected Animator m_Animator;

	public bool movable {
		get { return agent.enabled; }
		private set { 
			if (movable == value) return; 
			agent.enabled = value; 
		}
	}

	public Vector2 hitForce = new Vector2(-3, 1);

	// direction
	public virtual int direction {
		get { 
			return (int) Mathf.Sign(gameObject.transform.localScale.x);
		}
		
		protected set {
			if (direction == value) return;
			
			var _scale = gameObject.transform.localScale;
			_scale.x = value;
			gameObject.transform.localScale = _scale;
		}
	}

	public float? dyingDuration = 0.5f;
	public Vector2 deadForce = new Vector2(-3, 1);
	private bool m_IsDead = false;
	public bool isDead {
		get { return m_IsDead; }
		private set { m_IsDead = value; }
	}

	public HasDamage hasDamage;
	public DamageDetector damageDetector;

	public delegate void PostDead(object sender, EventArgs args);
	public event PostDead postDead;

	public virtual void Start () {

		// hp
		m_HP = GetComponent<HasHP>();
		m_HP.postDead += () => Die();
		
		m_Agent = GetComponent<Agent>();
		agent.direction = new Vector2(1, 0);
		agent.speed = 1;

		m_Animator = GetComponent<Animator>();
		if (m_Animator == null) {
			Debug.Log("Animator not set!");
		}

		direction = 1;

		// detector
		damageDetector.doDamage = Hit;
	}

	public virtual void OnDestroy() {

	}

	public virtual void Update() {

		if (agent.enabled 
		    && direction != Math.Sign(agent.direction.x)) 
		{
			direction = -direction;
		}

	}

	void Hit(AttackData attackData) {
		m_HP.damage(attackData);

		if (m_HP > 0) {

			var _hitForce = hitForce;
			_hitForce.x *= -Mathf.Sign(attackData.velocity.x);
			rigidbody2D.AddForce(_hitForce);
			
			m_Animator.SetTrigger("Hit");
		}
	}

	void Die() {

		if (isDead) return;

		if (m_HP > 0) {
			Debug.Log("Trying to kill the unit has hp!");
		}
		
		isDead = true;
		movable = false; 
		hasDamage.enabled = false;

		var _deadForce = deadForce;
		_deadForce.x *= direction;
		gameObject.rigidbody2D.AddForce(_deadForce);

		if (gameObject.collider2D != null) {
			gameObject.collider2D.enabled = false;
		}

		m_Animator.SetTrigger("Dead");

		if (postDead != null) {
			postDead(this, null);
		}
		
		if (dyingDuration != null) {
			Destroy(gameObject, dyingDuration.Value);
		} else {
			Destroy(gameObject);
		}
			
		Game.Statistic().mine.score.val += score;
	}
}
