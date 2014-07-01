using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class WeaponAimPivot : MonoBehaviour {

	void Start () {
	
	}
	
	void OnDrawGizmos () {
#if UNITY_EDITOR
		for (var _angle = -90; _angle <= 90; _angle += 15)
		{
			if (_angle % 45 == 0) {
				Gizmos.color = Color.blue;
			} else {
				Gizmos.color = Color.white;
			}

			var _angleRad = Mathf.Deg2Rad * _angle;

			Gizmos.DrawLine(transform.position, 
			                transform.position + 1.5f * new Vector3(Mathf.Cos(_angleRad), Mathf.Sin(_angleRad)));
		}
		
		for (var _radius = 1; _radius < 15; ++_radius) 
		{
			if (_radius % 5 == 0) {
				Handles.color = Color.blue;
			} else {
				Handles.color = Color.white;
			}

			Handles.DrawWireArc(
				transform.position,
				transform.forward,
				-transform.up,
				180,
				0.1f * _radius);
		}
#endif
	}
}
