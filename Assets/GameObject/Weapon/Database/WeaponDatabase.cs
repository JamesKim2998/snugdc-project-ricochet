using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponDatabase : MonoBehaviour
{
	public List<GameObject> editorDataPrfs;

	public Dictionary<WeaponType, WeaponData> m_Datas = new Dictionary<WeaponType, WeaponData>();

	public WeaponData this[WeaponType _type] {
		get {
			WeaponData data;
			if (m_Datas.TryGetValue(_type, out data))
			{
				return data;
			}
			else 
			{
				Debug.LogWarning("Trying to access " + _type + ", but data does not exist.");
				return null;
			}
		}
	}

	void Start()
	{
		foreach (var _dataPrf in editorDataPrfs)
		{
			var _type = _dataPrf.GetComponent<WeaponTypeComponent>();
			if (! _type) 
			{
				Debug.LogError("Type is not found!");
				continue;
			}
			
			var _dataCmp = _dataPrf.GetComponent<WeaponData>();
			if (! _dataCmp)
			{
				Debug.LogError("WeaponData is not found!");
				continue;
			}

			m_Datas.Add(_type.type, _dataCmp);
		}
	}
}

