
using UnityEngine;

public class SimpleGunDef : ScriptableObject {

	public Weapon weapon;
	public float speed;

	void OnEnable() {

		var _weapon = weapon as SimpleWeapon;

		if (_weapon == null) return;

		var _speed = speed;

		_weapon.doShoot = (_, _projectile) => {
			var _direction = new Vector2(0, 0);
			_direction.x = (_weapon.direction.x > 0) ? 1 : -1;
			_projectile.rigidbody2D.velocity = _speed * _direction;

			var _scale = _projectile.transform.localScale;
			_scale.x *= _direction.x;
			_projectile.transform.localScale = _scale;
		};
	}
	
}
