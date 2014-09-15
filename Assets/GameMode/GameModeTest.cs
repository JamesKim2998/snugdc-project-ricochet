using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameModeTestDef : GameModeDef
{
	public GameModeTestDef() { mode = GameModeType.TEST; } 
}

public class GameModeTest : GameMode
{
	bool m_IsLevelInited = false;

	public GameModeTest()
	{
		mode = GameModeType.TEST;
	}

	public override void Setup ()
	{
		base.Setup();

		// catch up game progress...
		if (Game.Progress.IsState(GameProgress.State.START))
		{
			ListenGameStart();
		}
		else if (Game.Progress.IsState(GameProgress.State.RUNNING))
		{
			ListenGameStart();
			ListenGameRun();
		}

		Game.Progress.postStart += ListenGameStart;
		Game.Progress.postRun += ListenGameRun;

		TryToRunGame();
	}

	public override void Init(GameModeDef _def)
	{
		base.Init(_def);
	}

	void OnDestroy()
	{
		Game.Progress.postStart -= ListenGameStart;
		Game.Progress.postRun -= ListenGameRun;
	}

    static void TryToRunGame()
	{
		if (Network.isServer)
		{
			if ( Game.Progress.IsState(GameProgress.State.STOP))
			{
				Game.Progress.StartGame();
			}
			else if (Game.Progress.IsState(GameProgress.State.START))
			{
				Game.Progress.RunGame();
			}
		}
	}
	
	void InitLevel() 
	{
		if (m_IsLevelInited) return;
		m_IsLevelInited = true;
	}

	void OnServerInitialized() 
	{
		TryToRunGame();
	}

	void ListenGameStart()
	{
		InitLevel();
		TryToRunGame();
	}

	void ListenGameRun()
	{
		if (! m_IsLevelInited)
			InitLevel();

		SpawnCharacter();
	}

	void SpawnCharacter()
	{
		var _character = Game.Level.characterSpawners.Spawn();

		if (_character != null) 
		{
			_character.postDead += ListenCharacterDead;
			Game.Character.character = _character;
		}
	}
	
	void ListenCharacterDead(Character _character)
	{
		Invoke("SpawnCharacter", 0.5f);
	}

}

