using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProjectileDatabase : MonoBehaviour
{
	public List<GameObject> editorDataPrfs;
	
	private readonly Dictionary<ProjectileType, ProjectileData> m_Datas = new Dictionary<ProjectileType, ProjectileData>();
	
	public ProjectileData this[ProjectileType _type] {
		get {
			ProjectileData _data;
			if (m_Datas.TryGetValue(_type, out _data))
			{
				return _data;
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
			var _type = _dataPrf.GetComponent<ProjectileTypeComponent>();
			if (! _type) 
			{
				Debug.LogError("Type is not found!");
				continue;
			}
			
			var _dataCmp = _dataPrf.GetComponent<ProjectileData>();
			if (! _dataCmp)
			{
				Debug.LogError("Data is not found!");
				continue;
			}
			
			m_Datas.Add(_type.type, _dataCmp);
		}
	}
}

