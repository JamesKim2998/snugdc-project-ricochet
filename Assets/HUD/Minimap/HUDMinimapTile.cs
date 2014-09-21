using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class HUDMinimapTileSearchPath
{
    public GameObject target;
    public bool dynamic = false;
}

[System.Serializable]
public class HUDMinimapTileInfo
{
    public Tile tile;
    public bool dynamic = false;
}

public class HUDMinimapTile : MonoBehaviour
{
    public List<HUDMinimapTileSearchPath> searchPathes;
    public List<HUDMinimapTileInfo> editorTiles;

    public HUDMinimapTileGizmo defaultGizmo;

	void Start () 
    {
	    foreach (var _searchPath in searchPathes)
	    {
	        foreach (var _go in GameObjectHelper.GetChildRecursive(_searchPath.target))
	        {
	            var _tile = ((GameObject) _go).GetComponent<Tile>();
                if (!_tile) continue;
                Add(_tile, _searchPath.dynamic);
	        }
	    }

	    foreach (var _tile in editorTiles)
	        Add(_tile.tile, _tile.dynamic);
	}

    void Add(Tile _tile, bool _dynamic)
    {
        if (!_tile.database) return;
        var _tileData = _tile.database[_tile.data];
        if (_tileData == null) return;

        var _gizmoPrf = _tileData.minimapGizmo ?? defaultGizmo;
        var _gizmo = (GameObject)Instantiate(_gizmoPrf.gameObject);
        TransformHelper.SetParentWithoutScale(_gizmo, gameObject);
        _gizmo.transform.localPosition = _tile.transform.position;

        var _gizmoCmp = _gizmo.GetComponent<HUDMinimapTileGizmo>();
        if (_gizmoCmp)
        {
            _gizmoCmp.Refresh(_tile, _tileData);

            var _size = _tile.data.sizeWorld;
            _size.x *= _tile.transform.localScale.x;
            _size.y *= _tile.transform.localScale.y;
            _gizmoCmp.SetSize(_size);
        }

        if (_dynamic)
        {
            var _updatable = _tile.GetComponent<Updatable>();
            if (!_updatable) goto dynamic_end;
            _updatable.postUpdate += delegate { _gizmo.transform.localPosition = _tile.transform.position; };
        } dynamic_end:;
    }
}
