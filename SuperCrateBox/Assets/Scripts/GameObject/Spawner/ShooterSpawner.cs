using UnityEngine;
using System.Collections;

public class ShooterSpawner : MonoBehaviour
{
	public GameObject shooter;

	void Start ()
	{

	}

	void Update ()
	{
		if (Game.Player().shooter == null) {
			var _gameObj = GameObject.Instantiate(shooter) as GameObject;
			_gameObj.transform.position = transform.position;
			Game.Player().shooter = _gameObj.GetComponent<Shooter>();

			var _shooter = _gameObj.GetComponent<Shooter>();
			_shooter.hitEnabled = false;
			_shooter.Invoke("EnableHit", 1.5f);
		}
	}
}

