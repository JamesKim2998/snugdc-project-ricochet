using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour {

	public GameObject target;

	public int maxUnits = 10;
	private int m_CurUnits = 0;
	public int curUnits { get {return m_CurUnits; }}

	public float periodMin;
	public float periodMax;
	public float periodOffset;

	public Vector2 velocity;

	public Rect range;
	public string[] filters;

	private bool m_CoroutineEnabled = false;
	public bool coroutineEnabled { get { return m_CoroutineEnabled; }}

	public bool networkEnabled = false;
	public bool networkServerOnly = true;

	public delegate void DoInitialize(Spawner _spawner, GameObject _gameObj);
	public DoInitialize doInitialize;

	public delegate void PostSpawn(Spawner _spawner, GameObject _gameObj);
	public event PostSpawn postSpawn;

	void Start () {
		if (! networkEnabled 
		    || (! networkServerOnly || Network.isServer))
		{
			StartSpawn();
		}

		MasterServerManager.postBeforeDisconnected += ListenBeforeDisconnected;
	}
	
	void Update () {
		
	}

	IEnumerator SpawnRoutine() {

		yield return new WaitForSeconds(periodOffset);

		while(true) 
		{
			if (curUnits >= maxUnits) 
			{
				yield return new WaitForSeconds(0.5f);
			} else 
			{
				Spawn();
				yield return new WaitForSeconds(Random.Range(periodMin, periodMax));
			}

		}
	}

	public void StartSpawn() {
		if (m_CoroutineEnabled) return;
		m_CoroutineEnabled = true;
		StartCoroutine("SpawnRoutine");
	}

	public void StopSpawn() {
		if (! m_CoroutineEnabled) return;
		m_CoroutineEnabled = false;
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
			var _serializable = _gameObj.GetComponent<ConstSerializable>();
			if (_serializable) _data = _serializable.Serialize();

			networkView.RPC("InstantiateNetwork", RPCMode.OthersBuffered, _gameObj.networkView.viewID, _data);
		}

		return _gameObj;
	}

	GameObject InstantiateLocal()
	{
		
		GameObject _gameObj;
		
		_gameObj = GameObject.Instantiate(
			target, 
			gameObject.transform.position, 
			gameObject.transform.rotation) 
			as GameObject;

		var _destroyable = _gameObj.GetComponent<Destroyable>();
		
		if (_destroyable) 
		{
			_destroyable.postDestroy += _ => {
				--m_CurUnits;
			};
		} 
		else 
		{
			Debug.Log("spawn non-destroyable object!");
		}

		if (doInitialize != null) 
		{
			doInitialize(this, _gameObj);
		}

		return _gameObj;
	}

	[RPC]
	void InstantiateNetwork(NetworkViewID _viewID, string _data) 
	{
		GameObject _gameObj = InstantiateLocal();
		_gameObj.networkView.viewID = _viewID;
		_gameObj.networkView.enabled = true;
		
		var _serializable = _gameObj.GetComponent<ConstSerializable>();
		if (_serializable) _serializable.Deserialize(_data);
	}

	public void Spawn() {

		if (target == null) {
			Debug.Log("trying to spawn empty spawner!");
			return;
		}

		GameObject _gameObj = Instantiate();

		++m_CurUnits;

		if ( postSpawn != null) {
			postSpawn(this, _gameObj);
		}
	}

	public Vector2 Locate() {

		int _hop = 5;

		while (_hop > 0) {
			
			var _position = new Vector2(
				Random.Range(range.xMin, range.xMax),
				Random.Range(range.yMin, range.yMax));
			
			_position += new Vector2(transform.position.x, transform.position.y);

			var _colliders = Physics2D.OverlapCircleAll(_position, 0.1f);

			var _available = ! System.Array.Exists(_colliders, collider => {
				return System.Array.Exists(filters, filter => filter == collider.tag);
			});

			if (_available) {
				return _position;
			}

			--_hop;
		}

		return transform.position;
	}

	void OnServerInitialized()
	{
		if (networkEnabled && networkServerOnly)
		{
			StartSpawn();
		}
	}

	void ListenBeforeDisconnected() 
	{
		Network.RemoveRPCs(networkView.viewID);
	}
}
