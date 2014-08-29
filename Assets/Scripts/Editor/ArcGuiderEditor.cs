using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ArcGuider))]
public class ArcGuiderEditor : Editor {
	
	ArcGuider m_This;
	
	public void OnEnable()
	{
		m_This = target as ArcGuider;
	}
	
	public void OnSceneGUI()
	{
		if (! m_This.enabled) return;
		
//		var _fromAngle = Mathf.Deg2Rad * (m_This.transform.parent.eulerAngles.z + 90 + m_This.from);
//		var _fromPosition = new Vector3(Mathf.Cos(_fromAngle), Mathf.Sin(_fromAngle), 0);

		Color _color;
		float _radius;

		if (m_This.radius < 2) 
		{
			_radius = m_This.radius;
			_color = Color.gray;
			_color.a = 0.1f;
		}
		else 
		{
			_radius = 2;
			_color = Color.blue;
			_color.a = 0.1f;
		}
		
		Handles.color = _color;

		Handles.DrawSolidArc(
			m_This.transform.position, 
			Vector3.forward, 
			m_This.transform.right,
			m_This.range, 
			_radius);
		
		Handles.DrawSolidArc(
			m_This.transform.position, 
			Vector3.forward, 
			m_This.transform.right,
			-m_This.range, 
			_radius);
	}
	
}
