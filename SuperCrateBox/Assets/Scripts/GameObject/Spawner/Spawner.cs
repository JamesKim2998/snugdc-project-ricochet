using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour {

	public GameObject[] targets;

	public int maxUnits = 10;
	private int m_CurUnits = 0;
	public int curUnits { get {return m_CurUnits; }}

	public float periodMin;
	public float periodMax;
	public float periodOffset;

	public Vector2 velocity;

	public Rect range;
	public string[] filters;

	public bool empty { get { return targets.Length == 0; }}

	public delegate void PostSpawn(Spawner _spawner, GameObject _gameObj);
	public event PostSpawn postSpawn;

	void Start () {
		StartCoroutine("SpawnRoutine");
	}
	
	void Update () {
		
	}

	IEnumerator SpawnRoutine() {

		while(true) {

			yield return new WaitForSeconds(periodOffset);

			if (curUnits >= maxUnits) {
				yield return new WaitForSeconds(0.5f);

			} else {

				Spawn();

				yield return new WaitForSeconds(Random.Range(periodMin, periodMax));
			}

		}
	}

	public void StartSpawn() {
		StartCoroutine("SpawnRoutine");
	}

	public void StopSpawn() {
		StopCoroutine("SpawnRoutine");
	}

	public void Spawn() {

		if (empty) {
			Debug.Log("trying to spawn empty spawner!");
			return;
		}

		var _target = targets[Random.Range(0, targets.Length)];

		var _gameObj = GameObject.Instantiate(
			_target, 
			gameObject.transform.position, 
			gameObject.transform.rotation) 
			as GameObject;

		_gameObj.transform.position = Locate();

		var _velocity = _gameObj.rigidbody2D.velocity;
		_velocity += velocity;

		var _destroyable = _gameObj.GetComponent<Destroyable>();

		if (_destroyable) {
			_destroyable.postDestroy += _ => {
				--m_CurUnits;
			};
		} else {
			Debug.Log("spawn non-destroyable object!");
		}

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
}
