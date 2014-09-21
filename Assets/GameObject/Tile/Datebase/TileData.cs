using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;

[Serializable]
public class TileDataKey
{
    public TileMaterialType material = TileMaterialType.NONE;

    public int width;
    public int height;
    public bool sloped = false;

    public Vector2 sizeWorld { get { return new Vector2(width / 2f, height / 2f); } }

    public TileDataKey(TileMaterialType _material, int _width, int _height, bool _sloped)
    {
        material = _material;
        width = _width;
        height = _height;
        sloped = _sloped;
    }

    public override string ToString()
    {
        return "( " + material + ", " + width + ", " + height + ", " + sloped + " )";
    }

    public override int GetHashCode()
    {
        return 13*(int)material + 17*width + 31*height + (sloped ? 37 : 0);
    }
}

[Serializable]
public class TileData
{
    public TileDataKey key;

    public TileColorType color = TileColorType.WHITE;

    public List<Sprite> sprite;
    public GameObject prefab;

    public HUDMinimapTileGizmo minimapGizmo;

    public override string ToString()
    {
        return key.ToString();
    }

    public override int GetHashCode()
    {
        return key.GetHashCode();
    }
}
