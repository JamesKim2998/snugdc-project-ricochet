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

    public Scene map = Scene.GAME_LEVEL_KKH_001;

	[HideInInspector]
	public GameModeDef modeSelected;
	public readonly Dictionary<GameModeType, GameModeDef> modes;
	public readonly GameModeDef modeNull = new GameModeDef();
	public readonly GameModeTestDef modeTest = new GameModeTestDef();
	public readonly GameModeDeathMatchDef modeDeathMatch = new GameModeDeathMatchDef();

}

