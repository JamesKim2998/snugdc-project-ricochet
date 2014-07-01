using UnityEngine;
using System.Collections;

public class JointPlacer : MonoBehaviour {
	
	public bool clockwise;

	public Bone root;
	public Bone joint;
	public Helper attachment;

	// Update is called once per frame
	void Update () 
	{
		var _root_position = Vector3.zero;
		var _attachment_position = root.transform.worldToLocalMatrix.MultiplyPoint3x4(attachment.transform.position);

		var _v = (_root_position - _attachment_position);
		var _d = _v.magnitude;
		var _l1 = root.length;
		var _l2 = joint.length;

		if (_d > _l1 + _l2) _d = _l1 + _l2;

		var _a1 = Mathf.Atan2(_v.x, -_v.y);
		var _a2 = Mathf.Acos((_d * _d + _l1 * _l1 - _l2 * _l2) / (2 * _d * _l1));

		var _a = _a1;
		_a += clockwise ? _a2 : -_a2;

		transform.position = root.transform.localToWorldMatrix.MultiplyPoint3x4(_l1 * new Vector3(-Mathf.Sin(_a), Mathf.Cos(_a), 0));
	}
}
