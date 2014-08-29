using UnityEngine;
using System.Collections;

public class FeetPlacer : MonoBehaviour {

	public Bone knee;
	public Bone feet;
	public Helper feetIK;

	public LayerMask terrain;

	public float positionLerp = 0.5f;
	public float angleLerp = 0.02f;

	public Vector2 positionFree;
	public float angleFree;

	void OnDisable()
	{
		var _angle = feet.transform.localEulerAngles;
		_angle.z = angleFree + 90;
		feet.transform.localEulerAngles = _angle;
	}

	void FixedUpdate() 
	{
		if (! enabled) return;

		bool overlapped = Physics2D.OverlapCircle(feet.transform.position, 0.1f, terrain) != null;

		Vector2 _position;
		float _zAngle;

		if (overlapped) 
		{
			var _raySource = new Vector2(feet.transform.position.x, knee.transform.position.y + 0.1f);
			var _rayDirection = new Vector2(0, -1);
			
			var _hit = Physics2D.Raycast(_raySource, _rayDirection, 1.5f, terrain);
			Debug.DrawRay(_raySource, 0.1f * _rayDirection, Color.red);
			
			if (_hit.collider == null) 
			{
				Debug.Log("feet placement failed!");
				return;
			}
			
			_position = _hit.point;
			_zAngle = Mathf.Rad2Deg * Mathf.Atan2(-_hit.normal.x, _hit.normal.y) + 270;
		} 
		else 
		{
			_position = knee.transform.position + new Vector3(positionFree.x, positionFree.y);
			_zAngle = angleFree + 270;
			// _positionLerp = 0.1f;
			// _angleLerp = 0.001f;
		}
		
		_position = Vector2.Lerp(feetIK.transform.position, _position, positionLerp);
		feetIK.transform.position = _position;

		var _angle = feet.transform.eulerAngles;
		_angle.z = Mathf.Lerp(_angle.z, _zAngle, angleLerp);
		feet.transform.eulerAngles = _angle;
		
	}
}
