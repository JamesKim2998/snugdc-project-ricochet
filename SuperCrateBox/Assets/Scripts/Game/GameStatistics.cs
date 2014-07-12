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
	public readonly Statistic<int> score;
	public readonly Statistic<int> death;
	public readonly string username;
	//public readonly Statistic<Weapon> weapon;

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
	private List<UserStatistic> userStatisticList;
	public GameStatistics() {
		userStatisticList = new List<UserStatistic> ();
	}
	public UserStatistic myUserStatistic {
		get {
			if (Game.Character ().character == null)
				return null;
			var _myNetworkPlayer = Game.Character ().character.networkView.owner;
			return userStatisticList.Find(el => el.player == _myNetworkPlayer);
		}
	}
	public void AddUserStatistic(NetworkPlayer player) {
		userStatisticList.Add(new UserStatistic(player));
	}
	public UserStatistic GetUserStatistic(NetworkPlayer player) {
		return userStatisticList.Find (el => el.player == player);
	}
}
