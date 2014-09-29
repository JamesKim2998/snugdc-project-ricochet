using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerStatistics 
{
	public readonly string player;

    public readonly ObservableValue<int> score = new ObservableValue<int>();

    public int CalculateScore()
    {
        var _balance = Game.Balance;
        score.val = _balance.score.kill * kill.val 
            + _balance.score.weaponPickUp * weaponPickUp.val;
        return score;
    }

    public readonly SetCounter<int> kill = new SetCounter<int>();
    public readonly SetCounter<int> death = new SetCounter<int>();
    public readonly SetCounter<int> suicide = new SetCounter<int>();
    public readonly SetCounter<int> consecutiveKill = new SetCounter<int>();
    public readonly SetCounter<int> consecutiveDeath = new SetCounter<int>();

    public readonly SetCounter<int> weaponPickUp = new SetCounter<int>();

	public PlayerStatistics(string _player) {
		player = _player;

	    kill.postAdd += delegate { CalculateScore(); };
		death.postAdd += (_, _characterID) => Game.Statistic.total.death.Add(_characterID);
	    weaponPickUp.postAdd += delegate { CalculateScore(); };

		Reset ();
	}

	public void Reset() {
		score.val = 0;
        kill.Clear();
		death.Clear();
        weaponPickUp.Clear();
	}
}

public class TotalStatistics 
{
	public readonly SetCounter<int> death = new SetCounter<int>();

	public TotalStatistics()
    {
    }

	public void Reset()
	{
		death.Clear();
	}
}

public class GameStatistics {
    public PlayerStatistics mine { get; private set; }
    private readonly Dictionary<string, PlayerStatistics> m_Statistics = new Dictionary<string, PlayerStatistics>();

    private readonly TotalStatistics m_Total = new TotalStatistics();
    public TotalStatistics total { get { return m_Total; } }

    public PlayerStatistics this[string _player] { get { return Get(_player); } }

    public void Start() {
		mine = new PlayerStatistics(Network.player.guid);

        mine.kill.postAdd += (_value, _change) =>
        {
            mine.consecutiveKill.Add(_change); 
            mine.consecutiveDeath.Clear();
        };

        mine.death.postAdd += (_value, _change) =>
        {
            mine.consecutiveDeath.Add(_change); 
            mine.consecutiveKill.Clear();
        };

		Global.Player().postConnected += ListenPlayerConnected;
	}

	public void OnDestroy() 
	{
		Global.Player().postConnected -= ListenPlayerConnected;
	}

	public PlayerStatistics Get(string _player) 
	{
		if (_player == Network.player.guid) return mine;
		if (m_Statistics.ContainsKey(_player))
		{
			return m_Statistics[_player];
		}
		else
		{
			Debug.Log("Statistic does not exist for player " + _player);
			return null;
		}
	}

	public void Add(string _player) 
	{
#if DEBUG
		if ( Get (_player) != null ) {
			Debug.LogError("Trying to add already existing player!");
		}
#endif

		m_Statistics[_player] = new PlayerStatistics(_player);
	}

	public void Purge() 
	{
		mine.Reset();
		m_Statistics.Clear();
	}

	public void Reset()
	{
		Debug.Log("Statistics reset");
		mine.Reset();
		foreach(var _statistics in m_Statistics)
			_statistics.Value.Reset();
		total.Reset();
	}

	void ListenPlayerConnected(bool _connected, string _player) 
	{
		if (_connected)
			Add (_player);
	}
}
