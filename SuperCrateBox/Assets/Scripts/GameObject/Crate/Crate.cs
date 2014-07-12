using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour {

	public string weapon;

	public bool empty { get { return weapon == null; } }

	public int score = 1;

	void Start() 
	{
		if (empty)
			Debug.Log("empty crate!");
	}

	void DestroySelf() 
	{
		if (networkView.enabled) 
		{
			Network.Destroy(gameObject);
		}
		else 
		{
			Destroy(gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D _collider)
	{
		if (_collider.gameObject.tag == "Character") 
		{
			var detector = _collider.gameObject.GetComponent<CrateDetector>();

			if (detector) 
			{
				if (! detector.enabled) return;
				detector.Obtain(this);
				Game.Statistic().myUserStatistic.score.val += score;
				DestroySelf();
			} 
			else 
			{
				Debug.Log("detector not found!");
			}

		}
	}

}