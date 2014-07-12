
using UnityEngine;

public class RocketLauncherDef : MonoBehaviour 
{
	public Weapon weapon;
	public float drivingForce;
	public float explosionRadius;

	void Start() 
	{
		var _weapon = weapon as SimpleWeapon;
		
		if (_weapon == null) return;
		
		var _drivingForce = drivingForce;

		_weapon.doShoot = (_, _projectile) => {
			_projectile.GetComponent<Projectile>().drivingForce = _drivingForce * _weapon.transform.right;
		};
	}
	
}
