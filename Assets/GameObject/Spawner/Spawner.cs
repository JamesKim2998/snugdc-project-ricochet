using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	public GameObject target;

	public int maxUnits = 10;
    public int curUnits { get; private set; }

    public float periodMin;
	public float periodMax;
	public float periodOffset;

	public Vector2 velocity;

	public Rect range;

    public bool coroutineEnabled { get; private set; }

    public bool networkEnabled = false;

	public delegate void DoInitialize(Spawner _spawner, GameObject _gameObj);
	public DoInitialize doInitialize;

	public delegate void PostSpawn(Spawner _spawner, GameObject _gameObj);
	public event PostSpawn postSpawn;

    public Spawner()
    {
        curUnits = 0;
        coroutineEnabled = false;
    }

    void Start () {
		if (! networkEnabled || (networkEnabled && Network.isServer))
			StartSpawn();

		Global.Server().postBeforeDisconnect += ListenBeforeDisconnected;
	}

    void OnDestroy()
    {
        Global.Server().postBeforeDisconnect -= ListenBeforeDisconnected;
    }

	IEnumerator SpawnRoutine() 
    {
		yield return new WaitForSeconds(periodOffset);
		Spawn();
		
		while(true) 
		{
			if (curUnits >= maxUnits) 
			{
				yield return new WaitForSeconds(0.5f);
			} 
            else
            {
                if (TestOverlap())
                {
                    yield return new WaitForSeconds(0.2f);
                    continue;
                }

                yield return new WaitForSeconds(Random.Range(periodMin, periodMax));
				Spawn();
			}

		}
	}

    bool TestOverlap()
    {
        var _colliders = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        return System.Array.Exists(_colliders, _collider => _collider.gameObject.name == target.tag);
    }

	public void StartSpawn() {
		if (coroutineEnabled) return;
		coroutineEnabled = true;
		StartCoroutine("SpawnRoutine");
	}

	public void StopSpawn() {
		if (! coroutineEnabled) return;
		coroutineEnabled = false;
		StopCoroutine("SpawnRoutine");
	}

	GameObject Instantiate() 
	{
		var _gameObj = InstantiateLocal();
		_gameObj.transform.position = Locate();
		
		var _velocity = _gameObj.rigidbody2D.velocity;
		_velocity += velocity;

		if (networkEnabled)
		{
			_gameObj.networkView.viewID = Network.AllocateViewID();
			_gameObj.networkView.enabled = true;

			string _data = "";
			var _serializable = _gameObj.GetComponent<StaticSerializable>();
			if (_serializable) _data = _serializable.Serialize();

			networkView.RPC("InstantiateNetwork", RPCMode.OthersBuffered, _gameObj.networkView.viewID, _data);
		}

		return _gameObj;
	}

	GameObject InstantiateLocal()
	{
		var _gameObj = (GameObject) Instantiate(
			target, 
			gameObject.transform.position, 
			gameObject.transform.rotation);

		var _destroyable = _gameObj.GetComponent<Destroyable>();
		
		if (_destroyable) 
			_destroyable.postDestroy += _ => --curUnits;
		else 
			Debug.Log("spawn non-destroyable object!");

		if (doInitialize != null) 
			doInitialize(this, _gameObj);

		return _gameObj;
	}

	[RPC]
	void InstantiateNetwork(NetworkViewID _viewID, string _data) 
	{
		var _gameObj = InstantiateLocal();
		_gameObj.networkView.viewID = _viewID;
		_gameObj.networkView.enabled = true;
		
		var _serializable = _gameObj.GetComponent<StaticSerializable>();
		if (_serializable) _serializable.Deserialize(_data);
	}

	public void Spawn() 
	{
		if (target == null) {
			Debug.Log("Target is not specified! Ignore.");
			return;
		}

		var _gameObj = Instantiate();

		++curUnits;

		if ( postSpawn != null) 
			postSpawn(this, _gameObj);
	}

	public Vector2 Locate() {

		var _hop = 5;

		while (_hop > 0) 
		{
			var _position = new Vector2(
				Random.Range(range.xMin, range.xMax),
				Random.Range(range.yMin, range.yMax)); //could be deleted
			
			_position += new Vector2(transform.position.x, transform.position.y);

			if (! TestOverlap()) 
				return _position;

			--_hop;
		}

		return transform.position;
	}

	void OnServerInitialized()
	{
		if (networkEnabled)
			StartSpawn();
	}

	void ListenBeforeDisconnected() 
	{
		Network.RemoveRPCs(networkView.viewID);
	}
}
