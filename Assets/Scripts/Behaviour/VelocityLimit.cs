using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class VelocityLimit : MonoBehaviour {

	public bool useHpos;
	public float hpos;

	public bool useHneg;
	public float hneg;

	public bool useVpos;
	public float vpos;

	public bool useVneg;
	public float vneg;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		var velocity = rigidbody2D.velocity;
		if (useHpos) velocity.x = Mathf.Min(velocity.x, hpos);
		if (useHneg) velocity.x = Mathf.Max(velocity.x, hneg);
		if (useVpos) velocity.y = Mathf.Min(velocity.y, vpos);
		if (useVneg) velocity.y = Mathf.Max(velocity.y, vneg);
		rigidbody2D.velocity = velocity;
	}
}
