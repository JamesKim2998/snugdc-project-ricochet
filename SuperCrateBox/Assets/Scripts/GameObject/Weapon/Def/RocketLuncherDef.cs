
using UnityEngine;

public class RocketLuncherDef : ScriptableObject {
	
	public Weapon weapon;
	public float drivingForce;
	public float explosionRadius;

	void OnEnable() {
		
		var _weapon = weapon as SimpleWeapon;
		
		if (_weapon == null) return;
		
		var _drivingForce = drivingForce;

		_weapon.doShoot = (_, _projectile) => {
			int direction = (_weapon.direction.x > 0) ? 1 : -1;

			_projectile.GetComponent<Projectile>().drivingForce = new Vector2(direction * _drivingForce, 0);
			
			var _scale = _projectile.transform.localScale;
			_scale.x *= direction;
			_projectile.transform.localScale = _scale;
		};
	}
	
}
