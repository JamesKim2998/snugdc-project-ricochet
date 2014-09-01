using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameSetting 
{
	public GameSetting()
	{
		modeSelected = modeDeathMatch;
		modes = new Dictionary<GameModeType, GameModeDef> {
			{ GameModeType.NULL, modeNull },
			{ GameModeType.TEST, modeTest },
			{ GameModeType.DEATH_MATCH, modeDeathMatch },
		};
	}

	public bool valid {
		get {
			return map != Scene.NONE
				&& modeSelected != null;
		}
	}

	public int mapIdx = 0;
	public Scene map { 
		get { 
			if (mapIdx < SceneInfos.gameMaps.Count)
				return SceneInfos.GameMap(mapIdx).scene;
			else
				return Scene.NONE; 
		}

		set {
			var _mapIdx = SceneInfos.gameMaps.FindIndex(_map => _map.scene == value);
			if (_mapIdx >= 0) 
				mapIdx = _mapIdx;
			else
				Debug.LogWarning("Map " + value + " is not found.");
		}
	}

	[HideInInspector]
	public GameModeDef modeSelected;
	public readonly Dictionary<GameModeType, GameModeDef> modes;
	public readonly GameModeDef modeNull = new GameModeDef();
	public readonly GameModeTestDef modeTest = new GameModeTestDef();
	public readonly GameModeDeathMatchDef modeDeathMatch = new GameModeDeathMatchDef();

}

