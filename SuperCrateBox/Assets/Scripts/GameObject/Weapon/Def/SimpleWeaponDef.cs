
using UnityEngine;

public class SimpleWeaponDef : ScriptableObject {

	public Weapon weapon;
	public GameObject projectile;

	void OnEnable() {

		var _weapon = weapon as SimpleWeapon;

		if (_weapon == null) return;

		var _projectilePrf = projectile;
		
		_weapon.doCreateProjectile = _ => {
			return (GameObject) GameObject.Instantiate(_projectilePrf);
		};
	}
	
}
