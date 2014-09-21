using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameModeTestDef : GameModeDef
{
	public GameModeTestDef() { type = GameModeType.TEST; } 
}

public class GameModeTest 
    : GameMode
    , GameModePropertyRespawn
    , GameModePropertyTimeLimit
{
	bool m_IsLevelInited = false;

    #region timelimit
    public int timeLimit { get; private set; }
    public int timeLeft
    {
        get
        {
            return Game.Progress.IsState(GameProgress.State.RUNNING)
                ? Mathf.Max(0, timeLimit - (int)Game.Progress.stateTime) : 0;
        }
    }
    #endregion timelimit

    #region respawn
    public int respawnLeft { get { return respawnLimit - Game.Statistic.total.death; } }
    public int respawnLimit { get; private set; }
    public int respawnDelay { get; private set; }
    #endregion respawn
    
    public GameModeTest()
	{
		type = GameModeType.TEST;
	    timeLimit = 300;
        respawnLimit = 10;
        respawnDelay = 5;
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

