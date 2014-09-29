using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameModeDeathMatchDef : GameModeDef
{
    public GameModeDeathMatchDef()
    {
        type = GameModeType.DEATH_MATCH;
    }

    private int? m_ResponseCount;
    public int respawnCount
    {
        get { return m_ResponseCount.HasValue ? m_ResponseCount.Value : defaultRespawnCount;  }
        set { m_ResponseCount = value; }
    }

    private int? m_ResponseDelay;
    public int respawnDelay
    {
        get { return m_ResponseDelay.HasValue ? m_ResponseDelay.Value : defaultRespawnDelay;  }
        set { m_ResponseDelay = value; }
    }

    public GameModeDeathMatchDef RespawnCount(int _var) { respawnCount = _var; return this; }

    private int? m_TimeLimit;
    public int timeLimit
    {
        get { return m_TimeLimit.HasValue ? m_TimeLimit.Value : defaultTimeLimit;  }
        set { m_TimeLimit = value; }
    }
    public GameModeDeathMatchDef TimeLimit(int _var) { timeLimit = _var; return this; }

    public static int defaultRespawnCount = 10;
    public static int defaultRespawnDelay = 3;
    public static int defaultTimeLimit = 300;
}

public class GameModeDeathMatch 
    : GameMode
    , GameModePropertyTimeLimit
    , GameModePropertyRespawn
{
	private bool m_IsLevelInited = false;

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
    #endregion

    #region respawn
    public int respawnLeft { get { return respawnLimit - Game.Statistic.total.death; } }
    public int respawnLimit { get; private set; }
    public int respawnDelay { get; private set; }
    #endregion

    public GameModeDeathMatch()
	{
		type = GameModeType.DEATH_MATCH;
	    timeLimit = 300;
	    respawnLimit = 10;
	    respawnDelay = 3;
	}

    void Start()
    {
        Global.Server().postServerInitialized += ListenServerInitialized;
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
		Game.Progress.postOver += ListenGameOver;
		Game.Statistic.total.death.postChanged += ListenTotalDeathChanged;
	    Game.Result.postPropagated += ListenResultPropagated;

		TryToRunGame();
	}

    void OnDestroy()
    {
        Global.Server().postServerInitialized -= ListenServerInitialized;
        Game.Progress.postStart -= ListenGameStart;
        Game.Progress.postRun -= ListenGameRun;
        Game.Progress.postOver -= ListenGameOver;
        Game.Statistic.total.death.postChanged -= ListenTotalDeathChanged;
        Game.Result.postPropagated -= ListenResultPropagated;
    }

	public override void Init(GameModeDef _def)
	{
		base.Init(_def);
        var _deathMatchDef = (GameModeDeathMatchDef) _def;
		respawnLimit = _deathMatchDef.respawnCount;
	    respawnDelay = _deathMatchDef.respawnDelay;
		timeLimit = _deathMatchDef.timeLimit;
	}

	void Update()
	{
		if (Game.Progress.IsState(GameProgress.State.RUNNING) 
		    && timeLeft <= 0)
		{
			Debug.Log("timeLimit exceeded.");

			if (Network.isServer)
			{
				if (! Game.Progress.TryOverGame()) 
					Debug.LogError("Over game failed.");
			}
		}
	}

    static void TryToRunGame()
	{
		if (Network.isServer )
		{
			if ( Game.Progress.IsState(GameProgress.State.STOP))
				Game.Progress.StartGame();
			else if (Game.Progress.IsState(GameProgress.State.START))
				Game.Progress.RunGame();
		}
	}
	
	void InitLevel() 
	{
		if (m_IsLevelInited) return;
		m_IsLevelInited = true;
	}
	
	void ListenServerInitialized() 
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
		Game.Character.character = null;
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
		if (Game.Progress.IsState(GameProgress.State.RUNNING))
			Invoke("SpawnCharacter", respawnDelay);
	}

	void ListenTotalDeathChanged(SetCounter<int> _statistic, int _change)
	{
		if (_statistic >= respawnLimit)
		{
			Debug.Log("respawnLimit over.");
			if (Network.isServer)
				Game.Progress.TryOverGame();
		}
	}

    void ListenResultPropagated()
    {
        var _statistics = StatisticManager.Instance;
        var _statisticMine = Game.Statistic.mine;
        var _resultMine = Game.Result.mine;

        // kill/death
        _statistics.totalKill.val += _resultMine.kill;
        _statistics.totalDeath.val += _resultMine.death;
        _statistics.totalSuicide.val += _statisticMine.suicide;

        if (_statistics.highestKill < _resultMine.kill)
            _statistics.highestKill.val = _statisticMine.kill;

        if (_statistics.highestDeath < _resultMine.death)
            _statistics.highestDeath.val = _statisticMine.death;

        if (_statistics.consecutiveKillCount < _statisticMine.consecutiveKill)
            _statistics.consecutiveKillCount.val = _statisticMine.consecutiveKill;

        if (_statistics.consecutiveDeathCount < _statisticMine.consecutiveDeath)
            _statistics.consecutiveDeathCount.val = _statisticMine.consecutiveDeath;

        // score
        if (_statistics.highscore < _resultMine.score)
            _statistics.highscore.val = _resultMine.score;
        _statistics.cumulativeScore.val += _resultMine.score;

        // misc
        _statistics.totalCratePickUp.val += _resultMine.weaponPickUp;
    }
}
