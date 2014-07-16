using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Statistic<T> where T : System.IEquatable<T> {
	private T m_Value;
	private T m_OldValue;

	public T val { 
		get { 
			return m_Value;
		} 

		set {
			m_OldValue = m_Value;
			m_Value = value;
			if (!m_Value.Equals(m_OldValue)
			    && postChanged != null) 
			{
				postChanged(this);
			}
		}
	}

	public T old { get { return m_OldValue; }}

	public delegate void PostChanged(Statistic<T> _statistic);
	public event PostChanged postChanged;

	public static implicit operator T(Statistic<T> _statistic) {
		return _statistic.val;
	}
}

public class UserStatistic {
	public NetworkPlayer player;
	public readonly string name;
	public readonly Statistic<int> score;
	public readonly Statistic<int> death;

	public UserStatistic(NetworkPlayer player) {
		this.player = player;
		score = new Statistic<int>();
		death = new Statistic<int>();
		Reset ();
	}

	public void Reset() {
		score.val = 0;
		death.val = 0;
	}
}

public class GameStatistics {
	private UserStatistic m_Mine;
	public UserStatistic mine { get { return m_Mine; }}
	private Dictionary<NetworkPlayer, UserStatistic> m_UserStatistics;

	public void Start() {
		m_Mine = new UserStatistic(Network.player);
		m_UserStatistics = new Dictionary<NetworkPlayer, UserStatistic> ();
	}
	
	public UserStatistic Get(NetworkPlayer player) {
		if (player == Network.player) return m_Mine;
		return m_UserStatistics[player];
	}

	public void Add(NetworkPlayer player) {
#if DEBUG
		if ( Get (player) != null ) {
			Debug.LogError("Trying to add already existing player!");
		}
#endif

		m_UserStatistics[player] = new UserStatistic(player);
	}

}
