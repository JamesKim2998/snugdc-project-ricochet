using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Explosion : MonoBehaviour {

	public AttackData attackData;
	public float radius;
	public float duration;

	public LayerMask targets;
	
	private bool m_Exploded = false;
	private float m_ExplosionTime = 0;

	void Start() {
		var _scale = transform.localScale;
		_scale.y = _scale.x = 0;
		transform.localScale = _scale;
	}

	void Update() {

		if (m_Exploded) {
			m_ExplosionTime += Time.deltaTime;
			var _scale = transform.localScale;
			_scale.y = _scale.x = 2 * radius * m_ExplosionTime / duration;
			transform.localScale = _scale;
		}

	}

	public void Explode() {

		if (m_Exploded) return;
		m_Exploded = true;

		var _colliders = Physics2D.OverlapCircleAll(transform.position, radius);

		foreach(Collider2D _collider in _colliders) {
			var _damageDetector = _collider.GetComponent<DamageDetector>();

			if (_damageDetector == null
			    || ! _damageDetector.enabled) 
			{
				continue;
			}
				
			if ((targets.value & _damageDetector.gameObject.layer) != 0) 
			{
				_damageDetector.Damage(attackData);
			}
		}

		Invoke("DestroySelf", duration);
	}

	void DestroySelf() {
		Destroy(gameObject);
	}
}
