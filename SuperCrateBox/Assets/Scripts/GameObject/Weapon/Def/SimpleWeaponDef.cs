
using UnityEngine;

public class SimpleWeaponDef : MonoBehaviour {

	public Weapon weapon;
	public string projectile;

	void Start() 
	{
		var _weapon = weapon as SimpleWeapon;

		if (_weapon == null) return;

		var _projectilePrf = projectile;
		
		_weapon.doCreateProjectile = _ => {
			return (GameObject) GameObject.Instantiate(Resources.Load(_projectilePrf));
		};

		_weapon.doCreateProjectileServer = (int _count, int _idx ) => {
			return (GameObject) GameObject.Instantiate(Resources.Load(_projectilePrf));
		};
	}
	
}
