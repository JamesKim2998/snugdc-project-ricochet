using UnityEngine;
using System.Collections;

public enum GameModeType 
{
	NULL,
	TEST,
}

[System.Serializable]
public class GameModeDef 
{
	public GameModeType mode = GameModeType.NULL;
}

public class GameMode : MonoBehaviour 
{
	public float setupDelay = -1f;
	public GameModeType mode = GameModeType.NULL;

	void Start()
	{
		if (setupDelay <= 0f) Setup();
		else Invoke("Setup", setupDelay);
	}

	public virtual void Setup()
	{
		if (Game.Mode() != null && Game.Mode() != this) Destroy(Game.Mode());
		Game.Instance.mode = this;
	}

	public virtual void Init(GameModeDef _def) 
	{ 
		mode = _def.mode;
		return; 
	}
	
	public static void Setup(GameModeDef _def)
	{
		if (Game.Mode() != null ) Destroy(Game.Mode());

		var _gameGO = Game.Instance.gameObject;
		GameMode _gameMode = null;

		switch (_def.mode)
		{
		case GameModeType.NULL: 
			Debug.Log("Trying to set game mode NULL!"); 
			_gameMode = _gameGO.AddComponent<GameMode>(); 
			break;

		case GameModeType.TEST: _gameMode = _gameGO.AddComponent<GameModeTest>(); break;
		}

		if (_gameMode == null)
		{
			Debug.LogError("GameMode for " + _def.mode.ToString() + " does not exist!");
			return;
		}

		_gameMode.Init(_def);
	}
}
