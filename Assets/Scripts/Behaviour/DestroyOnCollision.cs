using UnityEngine;
using System.Collections;

public class DestroyOnCollision : MonoBehaviour {

	public string[] ignores;

	void OnCollisionEnter2D (Collision2D _collision)
	{
		OnCollide(_collision.collider);
	}

	void OnTriggerEnter2D(Collider2D _collider)
	{
		OnCollide(_collider);
	}

	void OnCollide(Collider2D _collider)
	{
		
		if (System.Array.Exists(ignores, ignore => _collider.gameObject.tag == ignore)) 
			return;

		var _target = _collider.transform;

		while (_target != null)
		{
			var _collisionRoot = _target.GetComponent<DestroyOnCollisionRoot>();

			if (_collisionRoot == null) {
				_target = _target.transform.parent;
				continue;
			}

			Destroy(_collisionRoot.gameObject);
			return;
		}

		Debug.Log("DestroyOnCollisionRoot not found!");
	}

}
