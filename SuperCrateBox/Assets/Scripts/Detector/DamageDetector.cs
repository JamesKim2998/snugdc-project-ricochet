using UnityEngine;
using System.Collections;

public class DamageDetector : MonoBehaviour {

	public delegate void DoDamage(AttackData attackData);

	private DoDamage _doDamage;
	public DoDamage doDamage {
		set { _doDamage = value; }
	}

	public void Damage(AttackData attackData) {
		if (! enabled) return;

		if (_doDamage != null) {
			_doDamage(attackData);
		} else {
			Debug.Log("doDamage is not set!");
		}
	}

}
