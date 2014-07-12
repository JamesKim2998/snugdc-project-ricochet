using UnityEngine;
using System.Collections;

public class AlignToVelocity : MonoBehaviour {

	public Rigidbody2D body;
	public bool mirrorX = true;
	// public bool mirrorY = false;

	void Start()
	{
		if (body == null)
			body = rigidbody2D;
	}

	void Update () 
	{
		var _angle = transform.eulerAngles;
		_angle.y = 0;
		_angle.z = TransformHelper.VectorToDeg(body.velocity);

		if (mirrorX) 
		{
			if (body.velocity.x < 0) 
			{
				_angle.y = 180;
				_angle.z = 180 - _angle.z;
			}
		}

		transform.eulerAngles = _angle;
	}
}
