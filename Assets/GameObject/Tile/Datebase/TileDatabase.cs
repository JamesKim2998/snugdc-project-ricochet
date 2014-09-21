using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class TileDatabase : MonoBehaviour
{
    public List<TileData> editorTileDatas;
    public List<TileData> editorStoneDatas;
    public List<TileData> editorSteelDatas;
    public List<TileData> editorPaperDatas;
    public List<TileData> editorWoodenDatas;
    public List<TileData> editorLightOreDatas;

    private readonly Hashtable m_TileDatas = new Hashtable();

    void Awake()
    {
        List<TileData>[] _materialDatas = 
        {
            editorTileDatas,
            editorStoneDatas,
            editorSteelDatas,
            editorPaperDatas,
            editorWoodenDatas,
            editorLightOreDatas,
        };

        foreach (var _editorDatas in _materialDatas)
        {
            foreach (var _tileData in _editorDatas)
            {
#if DEBUG
                if (m_TileDatas.ContainsKey(_editorDatas.GetHashCode()))
                {
                    Debug.LogError("TileDatabase already have " + _editorDatas + ". Ignore.");
                    continue;
                }
#endif

                m_TileDatas.Add(_editorDatas.GetHashCode(), _editorDatas);
            }
        }
    }

    public TileData Search(TileMaterialType _material, int _width, int _height, bool _sloped = false)
    {
        var _hashkey = TileData.GetHashCode(_material, _width, _height, _sloped);

        if (!m_TileDatas.ContainsKey(_hashkey))
        {
            Debug.LogWarning("TileDatabase does not have (" 
                + _material + ", "+ _width + ", " + _height + ", " + _sloped + "). Return null.");
            return null;
        }

        return (TileData) m_TileDatas[_hashkey];
    }
}
