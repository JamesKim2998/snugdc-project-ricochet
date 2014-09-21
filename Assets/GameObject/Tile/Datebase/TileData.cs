using UnityEngine;
using System.Collections;

[System.Serializable]
public class TileData
{
    public TileMaterialType material = TileMaterialType.NONE;

    public int width;
    public int height;
    public bool sloped = false;

    public TileColorType color = TileColorType.WHITE;

    public Sprite[] sprite;
    public GameObject prefab;

    public GameObject minimapGizmo;

    public override string ToString()
    {
        return ToString(material, width, height, sloped);
    }

    public static string ToString(TileMaterialType _material, int _width, int _height, bool _sloped)
    {
        return "( " + _material + ", " + _width + ", " + _height + ", " + _sloped + " )";
    }

    public override int GetHashCode()
    {
        return GetHashCode(material, width, height, sloped);
    }

    public static int GetHashCode(TileMaterialType _material, int _width, int _height, bool _sloped)
    {
        return (int)_material * 11
            + _width * 23 + _height * 37
            + (_sloped ? 53 : 0);
    }
}
