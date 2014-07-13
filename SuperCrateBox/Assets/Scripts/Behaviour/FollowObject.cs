using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {

	public GameObject target;
	public Vector3 offset = Vector3.zero;
	public float lerp = 0.02f;
	public float mouseDeltaScale = 5.0f;

	void FixedUpdate () 
	{
		if (target == null) return;
		Vector3 mouseDelta = new Vector3 (Input.mousePosition.x - Screen.width / 2,
		                                Input.mousePosition.y - Screen.height / 2, 0);
		transform.position
			= Vector3.Lerp(transform.position, target.transform.position + offset + mouseDelta.normalized * mouseDeltaScale, lerp);
	}
}
