using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Crate))]
public class CrateEditor : Editor 
{
	Crate m_This;
	
	public void OnEnable()
	{
		m_This = target as Crate;
	}

	public void OnSceneGUI()
	{
		if (! m_This.enabled) return;
		Handles.Label(m_This.transform.position, m_This.weapon.ToString());
	}
}
