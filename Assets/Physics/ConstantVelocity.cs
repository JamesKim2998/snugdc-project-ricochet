using UnityEngine;
using System.Collections;

public class ConstantVelocity : MonoBehaviour {

	public Vector2 velocity;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		rigidbody2D.velocity = velocity;
	}
}
