using UnityEngine;
using System;
using System.Collections;

public enum GameModeType 
{
	NULL,
	TEST,
	DEATH_MATCH,
}

[System.Serializable]
public class GameModeDef 
{
	[HideInInspector]
	public GameModeType type = GameModeType.NULL;
	public bool overrideMode = false;
}

public class GameMode : MonoBehaviour 
{
	public GameModeType type = GameModeType.NULL;
	public bool overrideMode = false;

	public virtual void Setup() {}

	public virtual void Init(GameModeDef _def) 
	{ 
		type = _def.type;
		overrideMode = _def.overrideMode;
		return; 
	}
	
	public static GameModeDef CreateDef(GameModeType _mode)
	{
		switch ( _mode)
		{
		case GameModeType.NULL: 
			Debug.LogWarning("Trying to instantiate GameMode NULL.");
			return new GameModeDef();
			
		case GameModeType.TEST: return new GameModeTestDef();
		case GameModeType.DEATH_MATCH: return new GameModeDeathMatchDef();
			
		default: 
			Debug.LogWarning("Undefined mode " + _mode.ToString() + ".");
			return new GameModeDef();
		}
	}


}
