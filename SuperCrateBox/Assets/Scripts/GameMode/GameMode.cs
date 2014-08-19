using UnityEngine;
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
	public GameModeType mode = GameModeType.NULL;
	public bool overrideMode = false;
}

public class GameMode : MonoBehaviour 
{
	public GameModeType mode = GameModeType.NULL;

	public float setupDelay = -1f;
	public bool overrideMode = false;

	void Start()
	{
		if (setupDelay <= 0f) Setup();
		else Invoke("Setup", setupDelay);
	}

	public virtual bool Setup()
	{
		if (! overrideMode && (Game.Mode() != null) && (Game.Mode().mode != GameModeType.NULL)) return false;
		if (Game.Mode() != null && Game.Mode() != this) Destroy(Game.Mode());
		Game.Instance.mode = this;
		return true;
	}

	public virtual void Init(GameModeDef _def) 
	{ 
		mode = _def.mode;
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
	
	public static void Setup(GameModeDef _def)
	{
		if (Game.Mode() != null) Destroy(Game.Mode());

		var _gameGO = Game.Instance.gameObject;
		GameMode _gameMode = null;

		switch (_def.mode)
		{
		case GameModeType.NULL: 
			Debug.Log("Trying to set game mode NULL!"); 
			_gameMode = _gameGO.AddComponent<GameMode>(); 
			break;

		case GameModeType.TEST: _gameMode = _gameGO.AddComponent<GameModeTest>(); break;
		case GameModeType.DEATH_MATCH: _gameMode = _gameGO.AddComponent<GameMode>(); break;
			
		default: Debug.LogError("Unknown game mode."); break;
		}

		if (_gameMode == null)
		{
			Debug.LogError("GameMode for " + _def.mode.ToString() + " does not exist!");
			return;
		}

		_gameMode.Init(_def);
	}
}
