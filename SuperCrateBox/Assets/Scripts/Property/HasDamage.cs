using UnityEngine;
using System.Collections;

public class HasDamage : MonoBehaviour {

	public AttackData attackData;
	public string[] filters;
	
	void OnTriggerEnter2D (Collider2D _collider)
	{
		if (! enabled)
			return;

		if (filters.Length == 0) {
			Debug.Log("empty filter!");
			return;
		}

		if (System.Array.Exists(filters, filter => filter == _collider.gameObject.tag)) {
			var detector = _collider.GetComponent<DamageDetector>();

			if (detector) {
				detector.Damage(attackData);
			} else {
				Debug.Log("detector not found!");
			}
		}
	}
}
