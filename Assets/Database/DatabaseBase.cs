using System.Collections.Generic;
using System.Linq;
using Assets.Database;
using UnityEngine;
using System.Collections;

public class DatabaseBase<Type, TypeComponent, Data> : MonoBehaviour, IEnumerable<KeyValuePair<Type, Data>>
    where TypeComponent: UnityEngine.Component, IDatabaseTypeComponent<Type>
    where Data: UnityEngine.Component
{
    public List<GameObject> editorDataPrfs;

    protected Dictionary<Type, Data> m_Datas = new Dictionary<Type, Data>();

    public Data this[Type _type]
    {
        get
        {
            Data data;
            if (m_Datas.TryGetValue(_type, out data))
            {
                return data;
            }
            else
            {
                Debug.LogWarning("Trying to access " + _type + ", but data does not exist. Return null.");
                return default(Data);
            }
        }
    }

    void Start()
    {
        foreach (var _dataPrf in editorDataPrfs)
        {
            var _type = _dataPrf.GetComponent<TypeComponent>();
            if (!_type)
            {
                Debug.LogError("Type component is not found!");
                continue;
            }

            var _dataCmp = _dataPrf.GetComponent<Data>();
            if (!_dataCmp)
            {
                Debug.LogError("Data component is not found!");
                continue;
            }

            m_Datas.Add(_type.Type(), _dataCmp);
        }
    }

    public IEnumerator<KeyValuePair<Type, Data>> GetEnumerator()
    {
        return m_Datas.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return m_Datas.GetEnumerator();
    }
}
