using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponDatabase : MonoBehaviour
{
	public List<GameObject> editorWeaponDataPrfs;

	public Dictionary<WeaponType, WeaponData> m_WeaponDatas = new Dictionary<WeaponType, WeaponData>();

	void Start()
	{
		foreach (var _weaponData in editorWeaponDataPrfs)
		{
			var _type = _weaponData.GetComponent<WeaponTypeComponent>();
			if (! _type) 
			{
				Debug.LogError("Type is not found!");
				continue;
			}
			
			var _weaponDataCmp = _weaponData.GetComponent<WeaponData>();
			if (! _weaponDataCmp)
			{
				Debug.LogError("WeaponData is not found!");
				continue;
			}
			
			m_WeaponDatas.Add(_type.type, _weaponDataCmp);
		}
	}
}

