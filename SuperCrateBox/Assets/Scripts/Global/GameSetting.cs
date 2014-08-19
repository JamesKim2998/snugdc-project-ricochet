using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameSetting 
{
	public bool valid {
		get {
			return map != null
				&& mode != null;
		}
	}

	public int mapIdx = 0;
	public string map { 
		get { 
			if (mapIdx < maps.Count) 
			{
				return maps[mapIdx]; 
			}
			else
			{
				return null; 
			}
		}

		set {
			var _mapIdx = maps.FindIndex(_map => _map == value);
			if (_mapIdx >= 0) 
			{
				mapIdx = _mapIdx;
			}
			else
			{
				Debug.LogWarning("Map " + value + " is not found.");
			}
		}
	}

	public List<string> maps = new List<string>();

	public GameModeDef mode;
}

