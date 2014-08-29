using UnityEngine;
using System.Collections;

public class Laser : MonoBehaviour {

	public LayerMask reflectionMask;
	public int reflectionCount = 10;
	public float distance = 10;

	// ballet length * unitPerBalletLength = unity unit 1.
	public static int UNIT_PER_BALLET_LENGTH = 10;

	void Start()
	{
		rigidbody2D.velocity = Vector2.zero;

		if (reflectionCount > 0)
		{
			--reflectionCount;
			Stretch();
		}
	}

	void Stretch()
	{
		var _direction = transform.TransformDirection(Vector3.right);
		var _rayResults = Physics2D.RaycastAll(
			transform.position,
			_direction,
			Mathf.Infinity,
			reflectionMask);

		foreach (var _rayResult in _rayResults)
		{
			Vector2 _point = _rayResult.point;
			Vector2 _currentPoint = transform.position;
			Vector2 _diff = _currentPoint - _point;

			if (_diff.magnitude < 0.1) continue;

			var _magnitude = Mathf.Min(distance, _diff.magnitude);

			transform.position = _currentPoint + (Vector2)_direction * (_magnitude / 2);
			transform.localScale += new Vector3(_magnitude * UNIT_PER_BALLET_LENGTH, 0, 0);

			if (distance < _diff.magnitude) return;
			distance -= _magnitude;

			Laser _newLaser = Instantiate(this) as Laser;
			var _networkView = GetComponent<NetworkView>();
			if (_networkView != null) {
				// reflect is does not network syncronize.
				_networkView.enabled = false;
			}
			_newLaser.Reflect(_rayResult);

			return;
		}
		Debug.Log("RayCast failed!");
	}

	public void Reflect(RaycastHit2D _rayResult)
	{
		var _direction = transform.TransformDirection(Vector3.right);

		_direction = Vector3.Reflect(_direction, _rayResult.normal);
		var _angle = transform.eulerAngles;
		_angle.z = TransformHelper.VectorToDeg(_direction);
		transform.eulerAngles = _angle;

		transform.localScale = new Vector3(1, transform.localScale.y, transform.localScale.z);
		transform.position = _rayResult.point;
	}

}
