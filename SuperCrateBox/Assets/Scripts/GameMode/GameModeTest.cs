using UnityEngine;
using System.Collections;

public class GameModeTestDef : GameModeDef
{
	public string testLevel = "";
}

public class GameModeTest : GameMode
{
	public string testLevel = "";

	bool m_IsLevelInited = false;

	public GameModeTest()
	{
		mode = GameModeType.TEST;
	}

	public override void Setup ()
	{
		base.Setup();
		Game.Progress().postStart += ListenGameStart;
		Game.Progress().postRun += ListenGameRun;
		TryToRunGame();
	}

	public override void Init(GameModeDef _def)
	{
		base.Init(_def);
		testLevel = (_def as GameModeTestDef).testLevel;
	}

	void OnDestroy()
	{
		Game.Progress().postStart -= ListenGameStart;
		Game.Progress().postRun -= ListenGameRun;
	}

	void TryToRunGame()
	{
		if (Network.isServer && Application.loadedLevelName == testLevel)
		{
			if ( Game.Progress().IsState(GameProgress.State.STOP))
			{
				Game.Progress().StartGame();
			}
			else if (Game.Progress().IsState(GameProgress.State.START))
			{
				Game.Progress().RunGame();
			}
		}
	}
	
	void InitLevel() 
	{
		if (m_IsLevelInited) return;
		m_IsLevelInited = true;
		
		// todo: incomplete code
//		if (Network.isServer)
//			Game.Progress().StartGame();
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
		var _character = Game.Level().characterSpawners.Spawn();

		if (_character != null) 
		{
			_character.postDead += ListenCharacterDead;
			Game.Character().character = _character;
		}
	}
	
	void ListenCharacterDead(Character _character)
	{
		Invoke("SpawnCharacter", 0.5f);
	}

}

