using UnityEngine;
using System;
using System.Collections;

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
	public NetworkViewID networkId;
	public readonly Statistic<int> score;
	public readonly Statistic<int> death;
	public readonly string username;
	//public readonly Statistic<Weapon> weapon;

	public UserStatistic() {
		score = new Statistic<int>();
		death = new Statistic<int>();
	}
	
	public void Reset() {
		score.val = 0;
		death.val = 0;
	}
}
public class GameStatistics {
	public UserStatistic[] userStatisticList;
	public UserStatistic myUserStatistic {
		get {
			NetworkViewID myNetworkId = Game.Character ().character.networkView.viewID;
			return Array.Find(userStatisticList, el => el.networkId == myNetworkId);
		}
	}
}
