using UnityEngine;
using System.Collections;

public class HUDMinimapTileGizmo : MonoBehaviour
{
    public UISprite renderer_;

    void Awake()
    {
        ComponentHelper.AssignComponentIfNotExists(gameObject, ref renderer_);
    }

    public void Refresh(Tile _tile, TileData _data)
    {
        SetSize(_tile.data.sizeWorld);
        SetColor(_data.color);
    }

    public void SetSize(Vector2 _size)
    {
        if (renderer_)
        {
            var _scale = renderer_.transform.localScale;
            _scale.x = _size.x / 2;
            _scale.y = _size.y / 2;
            renderer_.transform.localScale = _scale;
        }
    }

    public void SetColor(TileColorType _color)
    {
        if (renderer_) 
            renderer_.color = TileDatabase.Convert(_color);
    }
}
