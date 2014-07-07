
using UnityEngine;

public class SimpleGunDef : ScriptableObject {

	public Weapon weapon;
	public float speed;

	void OnEnable() {

		var _weapon = weapon as SimpleWeapon;

		if (_weapon == null) return;

		var _speed = speed;

		_weapon.doShoot = (_, _projectile) => {
			_projectile.rigidbody2D.velocity = _speed * _weapon.transform.right;
		};
	}
	
}
