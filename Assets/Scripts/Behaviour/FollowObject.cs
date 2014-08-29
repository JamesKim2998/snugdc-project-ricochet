using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {

	public GameObject target;
	public Vector3 offset = Vector3.zero;
	public float lerp = 0.02f;

	void FixedUpdate () 
	{
		if (target == null) return;
		transform.position
			= Vector3.Lerp(transform.position, target.transform.position + offset, lerp);
	}
}
