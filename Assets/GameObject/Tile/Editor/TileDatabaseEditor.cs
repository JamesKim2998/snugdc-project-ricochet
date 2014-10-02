using UnityEditor;
using UnityEngine;
using System.Collections;

[CustomEditor(typeof(TileDatabase))]
public class TileDatabaseEditor : Editor
{
    private TileDatabase m_Target;

    public void OnEnable()
    {
        m_Target = (TileDatabase) target;
    }

    public override void OnInspectorGUI()
    {
	    base.OnInspectorGUI();

        if (GUILayout.Button("Rebuild"))
            m_Target.Rebuild();

        if (GUILayout.Button("Refresh all tiles"))
        {
            var _tiles = FindObjectsOfType<Tile>();
            foreach (var _tile in _tiles)
                _tile.Refresh();
        }
    }
	
}
