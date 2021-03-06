
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class SimpleLauncherDef : MonoBehaviour 
{
	public ProjectileType projectile;

	void Start() 
	{
		var _weapon = GetComponent<Weapon>();

#if DEBUG
		if (_weapon == null) 
		{
			Debug.LogError("Weapon is not exists! Ignore.");
			return;
		}
#endif
		
		var _projectilePrf = Database.Projectile[projectile].projectilePrf;

		_weapon.doCreateProjectile = delegate { return (GameObject) Instantiate(_projectilePrf); };

	    _weapon.doCreateProjectileServer = delegate { return (GameObject) Instantiate(_projectilePrf); };
	}
	
}
