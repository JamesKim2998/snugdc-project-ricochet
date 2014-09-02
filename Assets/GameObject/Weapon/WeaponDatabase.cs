using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponDatabase : MonoBehaviour
{
	public List<GameObject> editorWeaponDataPrfs;

	public Dictionary<WeaponType, WeaponData> m_WeaponDatas = new Dictionary<WeaponType, WeaponData>();

	public WeaponData this[WeaponType _type] {
		get {
			WeaponData data;
			if (m_WeaponDatas.TryGetValue(_type, out data))
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
		foreach (var _weaponDataPrf in editorWeaponDataPrfs)
		{
			var _type = _weaponDataPrf.GetComponent<WeaponTypeComponent>();
			if (! _type) 
			{
				Debug.LogError("Type is not found!");
				continue;
			}
			
			var _weaponDataCmp = _weaponDataPrf.GetComponent<WeaponData>();
			if (! _weaponDataCmp)
			{
				Debug.LogError("WeaponData is not found!");
				continue;
			}

			Debug.Log("Weapon data " + _type.type + " added.");
			m_WeaponDatas.Add(_type.type, _weaponDataCmp);
		}
	}
}

