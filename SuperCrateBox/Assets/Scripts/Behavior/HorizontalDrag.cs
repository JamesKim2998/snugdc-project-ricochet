using UnityEngine;
using System.Collections;

public class HorizontalDrag : MonoBehaviour {
	public float drag = 0;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		var body = gameObject.rigidbody2D;
		var speed = body.velocity.magnitude;
		var dragForce = (speed * speed) * drag;
		var normal = body.velocity.normalized;
		body.AddForce(new Vector2(dragForce * -normal.x, 0));
	}	
}
