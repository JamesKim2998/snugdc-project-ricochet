using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(AngleConstraint))]
public class AngleConstraintEditor : Editor {

	AngleConstraint m_This;

	public void OnEnable()
	{
		m_This = target as AngleConstraint;
	}

	public override void OnInspectorGUI()
	{
		EditorGUILayout.LabelField("angle: " + (m_This.transform.localEulerAngles.z));

		m_This.from = EditorGUILayout.IntField("from", m_This.from);
		m_This.range = EditorGUILayout.IntField("range", m_This.range);

		EditorUtility.SetDirty(m_This);
	}

	public void OnSceneGUI()
	{
		if (! m_This.enabled) return;

		var _fromAngle = Mathf.Deg2Rad * (m_This.transform.parent.eulerAngles.z + 90 + m_This.from);
		var _fromPosition = new Vector3(Mathf.Cos(_fromAngle), Mathf.Sin(_fromAngle), 0);

		var color = Color.green;
		color.a = 0.2f;
		Handles.color = color;

		Handles.DrawSolidArc(
			m_This.transform.position, 
			Vector3.forward, 
			_fromPosition,
			m_This.range, 
			0.1f);
	}

}
