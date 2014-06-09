using UnityEngine;
using System.Collections;

public class DestroyOnCollision : MonoBehaviour {

	public string[] ignores;

	void OnTriggerEnter2D(Collider2D _collider)
	{
		if (System.Array.Exists(ignores, ignore => _collider.gameObject.tag == ignore)) 
		    return;

		Destroy(_collider.gameObject);
	}
}
