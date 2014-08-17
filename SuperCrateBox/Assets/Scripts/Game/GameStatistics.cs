using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerStatistics 
{
	public readonly string player;
	public readonly Statistic<int> score;
	public readonly Statistic<int> death;

	public PlayerStatistics(string _player) {
		player = _player;
		score = new Statistic<int>();
		death = new Statistic<int>();

		death.postChanged += (Statistic<int> statistic) => {
			++Game.Statistic ().total.death.val;
		};

		Reset ();
	}

	public void Reset() {
		score.val = 0;
		death.val = 0;
	}
}

// todo: incomplete code
public class TotalStatistics 
{
	public readonly Statistic<int> death;

	public TotalStatistics() 
	{
		death = new Statistic<int>();
	}
}

public class GameStatistics {
	private PlayerStatistics m_Mine;
	public PlayerStatistics mine { get { return m_Mine; }}
	private Dictionary<string, PlayerStatistics> m_Statistics;

	private TotalStatistics m_Total;
	public TotalStatistics total { get { return m_Total; } }

	public void Start() {
		m_Mine = new PlayerStatistics(Network.player.guid);
		m_Total = new TotalStatistics ();
		m_Statistics = new Dictionary<string, PlayerStatistics> ();
		Global.Player().postConnected += ListenPlayerConnected;
	}

	public void OnDestroy() 
	{
		Global.Player().postConnected -= ListenPlayerConnected;
	}

	public PlayerStatistics Get(string _player) 
	{
		if (_player == Network.player.guid) return m_Mine;
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
		m_Mine.Reset();
		m_Statistics.Clear();
	}

	void ListenPlayerConnected(bool _connected, string _player) 
	{
		if (_connected)
			Add (_player);
	}
}
