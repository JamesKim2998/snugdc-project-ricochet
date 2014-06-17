using UnityEngine;
using System.Collections;

public class CrateSpawner : MonoBehaviour {
	
	void Start () {

		var _spawner = GetComponent<Spawner>();

		_spawner.doInitialize = (Spawner _theSpawner, GameObject _gameObj) => {
			var _crate = _gameObj.GetComponent<Crate>();
			_crate.weapon = Game.Weapon().Random();
		};
	}
}
