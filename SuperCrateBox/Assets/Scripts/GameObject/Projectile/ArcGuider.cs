using UnityEngine;
using System.Collections;

public class ArcGuider : MonoBehaviour 
{
	public GameObject target;

	public int range = 180;
	public float radius;

	public float constCatchUp = 5;
	public float lerpCatchUp = 0.02f;

	void FixedUpdate () 
	{
		if (target == null)
			return;

		var _targetPosition = transform.worldToLocalMatrix.MultiplyPoint3x4(target.transform.position);

		if (_targetPosition.sqrMagnitude > radius * radius)
			return;

		var _targetAngle = TransformHelper.VectorToDeg(_targetPosition);

		if (_targetAngle > range || _targetAngle < -range )
		    return;

		var _newAngle = transform.localEulerAngles;
		_newAngle.z += Mathf.Clamp(_targetAngle, -constCatchUp, constCatchUp);
		_newAngle.z += Mathf.Lerp(0, _targetAngle, lerpCatchUp);
		transform.localEulerAngles = _newAngle;
	}
}
