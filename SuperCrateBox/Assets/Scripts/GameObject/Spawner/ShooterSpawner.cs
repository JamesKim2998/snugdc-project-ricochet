using UnityEngine;
using System.Collections;

public class ShooterSpawner : MonoBehaviour
{
	public GameObject shooterPrf;

	public Rect spawnRange;

	void Start ()
	{

	}

	void Update ()
	{
		if (Game.Player().shooter == null) {
			var _shooter = Spawn();
			Game.Player().shooter = _shooter;
			
			_shooter.hitEnabled = false;
			_shooter.Invoke("EnableHit", 1.5f);
		}
	}

	Shooter Spawn() 
	{
		Vector3 _shooterPosition = Locate();

		var _gameObj = GameObject.Instantiate(shooterPrf, _shooterPosition, Quaternion.identity) as GameObject;

		var _destroyable = _gameObj.GetComponent<Destroyable>();
		_destroyable.postDestroy += ListenDestroy;

		var _shooter = _gameObj.GetComponent<Shooter>();

		if (networkView.enabled && Network.peerType != NetworkPeerType.Disconnected)
		{
			_shooter.networkView.viewID = Network.AllocateViewID();
			_shooter.networkView.enabled = true;
			
			var _observed = _gameObj.AddComponent<InterpolatePosition>();
			_shooter.networkView.observed = _observed;
			
			networkView.RPC("SpawnServer", RPCMode.OthersBuffered, _shooter.networkView.viewID, _shooterPosition);// _shooter.transform.position);
		}

		return _shooter;
	}

	Vector2 Locate() 
	{
		Vector2 _position = Vector2.zero;
		_position.x = transform.position.x + spawnRange.xMin + Random.Range(0, spawnRange.width);
		_position.y = transform.position.y + spawnRange.yMin + Random.Range(0, spawnRange.height);
		return _position;
	}

	[RPC]
	void SpawnServer(NetworkViewID _viewID, Vector3 _position)
	{
		var _shooter = GameObject.Instantiate(shooterPrf, _position, Quaternion.identity) as GameObject;

		_shooter.networkView.enabled = true;
		_shooter.networkView.viewID = _viewID;

		var _observed = _shooter.AddComponent<InterpolatePosition>();
		_shooter.networkView.observed = _observed;
	}

	void ListenDestroy(Destroyable _destroyable)
	{
		if (networkView.enabled) {
			Network.RemoveRPCs(networkView.viewID);
		}
	}
}

