using UnityEditor;
using UnityEngine;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(Tile))]
public class TileEditor : Editor 
{
	private Tile m_This;
	
	public void OnEnable()
	{
		m_This = (Tile) target;
        m_This.Setup();
	}

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Refresh"))
            m_This.Refresh();
    }
}
