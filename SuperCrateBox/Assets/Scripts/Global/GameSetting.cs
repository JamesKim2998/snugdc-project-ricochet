using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameSetting 
{
	public GameSetting()
	{
		modeSelected = modeTest;
		modes = new Dictionary<GameModeType, GameModeDef> {
			{ GameModeType.NULL, modeNull },
			{ GameModeType.TEST, modeTest },
			{ GameModeType.DEATH_MATCH, modeDeathMatch },
		};
	}

	public bool valid {
		get {
			return map != null
				&& modeSelected != null;
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
	
	public GameModeDef modeSelected;
	public readonly Dictionary<GameModeType, GameModeDef> modes;
	public readonly GameModeDef modeNull = new GameModeDef();
	public readonly GameModeTestDef modeTest = new GameModeTestDef();
	public readonly GameModeDeathMatchDef modeDeathMatch = new GameModeDeathMatchDef();

}

