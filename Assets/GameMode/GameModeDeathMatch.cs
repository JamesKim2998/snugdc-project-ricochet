using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameModeDeathMatchDef : GameModeDef
{
	public GameModeDeathMatchDef() { mode = GameModeType.DEATH_MATCH; } 

	public int respawnCount = 10;
	public GameModeDeathMatchDef RespawnCount(int _var) { respawnCount = _var; return this; }

	public int timeLimit = 300;
	public GameModeDeathMatchDef TimeLimit(int _var) { timeLimit = _var; return this; }
}

public class GameModeDeathMatch : GameMode
{
	private bool m_IsLevelInited = false;

	public int respawnLimit = 10;
	public int timeLimit = 300;

	public int respawnLeft { get { return respawnLimit - Game.Statistic.total.death; } }
	public int timeLeft { 
		get { 
			if (Game.Progress().IsState(GameProgress.State.RUNNING))
			{
				return Mathf.Max(0, timeLimit - (int) Game.Progress().stateTime); 
			}
			else
			{
				return 0;
			}
		} 
	}

	public GameModeDeathMatch()
	{
		mode = GameModeType.DEATH_MATCH;
	}
	
	public override void Setup ()
	{
		base.Setup();
		
		// catch up game progress...
		if (Game.Progress().IsState(GameProgress.State.START))
		{
			ListenGameStart();
		}
		else if (Game.Progress().IsState(GameProgress.State.RUNNING))
		{
			ListenGameStart();
			ListenGameRun();
		}
		
		Game.Progress().postStart += ListenGameStart;
		Game.Progress().postRun += ListenGameRun;
		Game.Progress().postOver += ListenGameOver;
		Game.Statistic.total.death.postChanged += ListenTotalDeathChanged;

		TryToRunGame();
	}
	
	public override void Init(GameModeDef _def)
	{
		base.Init(_def);
        var _deathMatchDef = (GameModeDeathMatchDef) _def;
		respawnLimit = _deathMatchDef.respawnCount;
		timeLimit = _deathMatchDef.timeLimit;
	}

	void Update()
	{
		if (Game.Progress().IsState(GameProgress.State.RUNNING) 
		    && timeLeft <= 0)
		{
			Debug.Log("timeLimit exceeded.");

			if (Network.isServer)
			{
				if (! Game.Progress().TryOverGame()) 
					Debug.LogError("Over game failed.");
			}
		}
	}

	void OnDestroy()
	{
		Game.Progress().postStart -= ListenGameStart;
		Game.Progress().postRun -= ListenGameRun;
		Game.Progress().postOver -= ListenGameOver;
		Game.Statistic.total.death.postChanged -= ListenTotalDeathChanged;
	}
	
	void TryToRunGame()
	{
		if (Network.isServer )
		{
			if ( Game.Progress().IsState(GameProgress.State.STOP))
				Game.Progress().StartGame();
			else if (Game.Progress().IsState(GameProgress.State.START))
				Game.Progress().RunGame();
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

	void ListenGameOver()
	{
		CancelInvoke("SpawnCharacter");
		Game.Character ().character = null;
	}

	void SpawnCharacter()
	{
//		Debug.Log("SpawnCharacter");
		var _character = Game.Level().characterSpawners.Spawn();
		
		if (_character != null) 
		{
			_character.postDead += ListenCharacterDead;
			Game.Character().character = _character;
		}
	}
	
	void ListenCharacterDead(Character _character)
	{
//		Debug.Log("Character Dead");
		if (Game.Progress().IsState(GameProgress.State.RUNNING))
			Invoke("SpawnCharacter", 0.5f);
	}

	void ListenTotalDeathChanged(Statistic<int> _statistic)
	{
		if (_statistic >= respawnLimit)
		{
			Debug.Log("respawnLimit over.");
			if (Network.isServer)
				Game.Progress().TryOverGame();
		}
	}
}
