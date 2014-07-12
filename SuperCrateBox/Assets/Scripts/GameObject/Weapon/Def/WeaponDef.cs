
using UnityEngine;

public class WeaponDef : MonoBehaviour {
	
	public Weapon weapon;
	public float cooldown;

	public bool useDamage = false;
	public int damage;

	void Start()
	{
		weapon.cooldown = cooldown;

		if (useDamage) {
			weapon.damage = damage;
		}
	}

}
