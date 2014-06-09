using UnityEngine;
using System.Collections;

public class DrivingForce : MonoBehaviour {

	public Vector2 force;
	
	void Update () {
		rigidbody2D.AddForce(force);
	}
}
