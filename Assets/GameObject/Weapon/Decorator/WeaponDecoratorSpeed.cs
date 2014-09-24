
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class WeaponDecoratorSpeed : MonoBehaviour 
{
	public float speed;

	void Start() 
	{
		var _weapon = GetComponent<Weapon>();

		if (_weapon == null) return;

		var _speed = speed;

		_weapon.doShootMine = (_, _projectile) => {
			_projectile.rigidbody2D.velocity = _speed * _weapon.transform.right;
		};
	}
	
}
