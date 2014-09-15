using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerStatistics 
{
	public readonly string player;
    public readonly Statistic<int> score = new Statistic<int>();
    public readonly SetCounter<int> kill = new SetCounter<int>();
    public readonly SetCounter<int> death = new SetCounter<int>();

	public PlayerStatistics(string _player) {
		player = _player;
        
		death.postAdd += (_, _characterID) => Game.Statistic.total.death.Add(_characterID);
		death.postRemove += (_, _characterID) => Game.Statistic.total.death.Remove(_characterID);

		Reset ();
	}

	public void Reset() {
		score.val = 0;
		death.Clear();
	}
}

// todo: incomplete code
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
