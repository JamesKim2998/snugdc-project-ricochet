
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class SimpleWeaponDef : MonoBehaviour 
{
	public string projectile;

	void Start() 
	{
		var _weapon = GetComponent<Weapon>();

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
