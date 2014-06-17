
using UnityEngine;

public class WeaponDef : ScriptableObject {
	
	public Weapon weapon;
	public float cooldown;

	public bool useDamage = false;
	public int damage;

	void OnEnable ()
	{
		weapon.cooldown = cooldown;

		if (useDamage) {
			weapon.damage = damage;
		}
	}

}
