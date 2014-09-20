using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Spawner))]
public class CrateSpawner : MonoBehaviour 
{
	void Start () 
	{
		var _spawner = GetComponent<Spawner>();

		_spawner.doInitialize = (_theSpawner, _gameObj) => {
			var _crate = _gameObj.GetComponent<Crate>();
			_crate.weapon = Game.Weapon.Random();
		};
	}
}
