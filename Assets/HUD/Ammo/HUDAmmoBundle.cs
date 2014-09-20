using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HUDAmmoBundle : MonoBehaviour
{
    public HUDAmmoData ammoData;
    private readonly List<GameObject> m_Bundle = new List<GameObject>();

    public int count
    {
        get { return m_Bundle.Count; }
        set
        {
            while (count < value)
                Add();
            while (count > value)
                Remove();
        }
    }

    public Vector2 GetOffset()
    {
        var _row = count % ammoData.line;
        var _column = count / ammoData.line;
        var _positionX = _column*(ammoData.size.x + ammoData.padding.x*2) + ammoData.padding.x;
        var _positionY = _row*(ammoData.size.y + ammoData.padding.y*2);
        return new Vector2(_positionX, _positionY);
    }

    public float GetNextColumnX()
    {
        var x = GetOffset().x;
        if (count%ammoData.line != 0)
            x += ammoData.size.x + ammoData.padding.x * 2;
        return x;
    }

    void Add()
    {
        var _ammo = (GameObject)Instantiate(ammoData.gameObject);
        TransformHelper.SetParentWithoutScale(_ammo, gameObject);
        _ammo.transform.localPosition = GetOffset();
        m_Bundle.Add(_ammo);
    }

    void Remove()
    {
        Destroy(m_Bundle.Last());
        m_Bundle.RemoveAt(m_Bundle.Count - 1);
    }

    void Clear()
    {
        foreach (var _ammo in m_Bundle)
            Destroy(_ammo);
        m_Bundle.Clear();
    }

}
