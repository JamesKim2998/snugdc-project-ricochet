using System.Collections.Generic;
using System.Text;
using UnityEngine;
using System.Collections;

public class TileDatabase : MonoBehaviour
{
    public List<TileData> editorTileDatas;
    public List<TileData> editorStoneDatas;
    public List<TileData> editorSteelDatas;
    public List<TileData> editorPaperDatas;
    public List<TileData> editorWoodenDatas;

    private readonly Hashtable m_TileDatas = new Hashtable();

    void Awake()
    {
        Rebuild();
    }

    public TileData this[TileDataKey _key]
    {
        get
        {
            if (m_TileDatas.Count == 0)
                Rebuild();

            var _hashkey = _key.GetHashCode();

            if (!m_TileDatas.ContainsKey(_hashkey))
            {
                Debug.LogWarning("TileDatabase does not have " + _key + ". Return null.");
                return null;
            }

            return (TileData)m_TileDatas[_hashkey];
        }
    }

    public void Rebuild()
    {
        m_TileDatas.Clear();

        List<TileData>[] _materialDatas = 
        {
            editorTileDatas,
            editorStoneDatas,
            editorSteelDatas,
            editorPaperDatas,
            editorWoodenDatas,
        };

        foreach (var _editorDatas in _materialDatas)
        {
            foreach (var _tileData in _editorDatas)
            {
#if DEBUG
                if (m_TileDatas.ContainsKey(_tileData.GetHashCode()))
                {
                    Debug.LogError("TileDatabase already have " + _tileData + ". Ignore.");
                    continue;
                }
#endif

                m_TileDatas.Add(_tileData.GetHashCode(), _tileData);
            }
        }
    }

    public static Color Convert(TileColorType _color)
    {
        switch (_color)
        {
        case TileColorType.WHITE:   return Color.white;
        case TileColorType.GRAY:    return Color.gray;
        case TileColorType.BLACK:   return Color.black;
        case TileColorType.RED:     return Color.red;
        case TileColorType.YELLOW:  return Color.yellow;
        case TileColorType.GREEN:   return Color.green;
        case TileColorType.CYAN:    return Color.cyan;
        case TileColorType.BLUE:    return Color.blue;
        case TileColorType.MAGENTA: return Color.magenta;
        default:
                Debug.LogWarning("Unknown TileColorType" + _color + ". Return black.");
                return Color.black;
        }
    }
}
