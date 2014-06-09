using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour {

	public GameObject weapon;

	public bool empty { get { return weapon == null; } }

	public int score = 1;

	void Start() {

		if (empty) {
			Debug.Log("empty crate!");
		}

	}

	void OnCollisionEnter2D(Collision2D _collider)
	{
		if (_collider.gameObject.tag == "Player") {

			var detector = _collider.collider.gameObject.GetComponent<CrateDetector>();

			if (detector) {
				detector.doObtain(this);
				Game.Statistic().score.val += score;

				Destroy(gameObject);

			} else {
				Debug.Log("detector not found!");
			}


		}
	}
}