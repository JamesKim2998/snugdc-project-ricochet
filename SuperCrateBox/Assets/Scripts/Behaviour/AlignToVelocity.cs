using UnityEngine;
using System.Collections;

public class AlignToVelocity : MonoBehaviour {

	public bool mirrorX = true;
	// public bool mirrorY = false;

	void Update () 
	{
		var _angle = transform.eulerAngles;
		_angle.y = 0;
		_angle.z = TransformHelper.VectorToDeg(rigidbody2D.velocity);

		if (mirrorX) 
		{
			if (rigidbody2D.velocity.x < 0) 
			{
				_angle.y = 180;
				_angle.z = 180 - _angle.z;
			}
		}

		transform.eulerAngles = _angle;
	}
}
