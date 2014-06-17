using UnityEngine;
using System.Collections;

public class DamageDetector : MonoBehaviour {

	public delegate void DoDamage(AttackData attackData);

	private DoDamage m_DoDamage;
	public DoDamage doDamage {
		set { m_DoDamage = value; }
	}

	public void Damage(AttackData attackData) {
		if (! enabled) 
		{
			Debug.Log("trying to damage disabled DamageDetector!");
			return;
		}

		if (m_DoDamage != null) 
		{
			m_DoDamage(attackData);
		} 
		else 
		{
			Debug.Log("doDamage is not set!");
		}
	}

}
