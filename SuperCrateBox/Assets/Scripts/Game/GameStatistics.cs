using UnityEngine;
using System.Collections;

public class Statistic {
	private int m_Value = 0;
	private int m_OldValue = 0;

	public int val { 
		get { 
			return m_Value;
		} 

		set {
			m_OldValue = m_Value;
			m_Value = value; 
			
			if (m_Value != m_OldValue
			    && postChanged != null) 
			{
				postChanged(this);
			}
		}
	}

	public int old { get { return m_OldValue; }}

	public delegate void PostChanged(Statistic _statistic);
	public event PostChanged postChanged;

	public static implicit operator int(Statistic _statistic) {
		return _statistic.val;
	}

}

public class GameStatistics {
	public readonly Statistic score;
	public readonly Statistic death;

	public GameStatistics() {
		score = new Statistic();
		death = new Statistic();
	}

	public void Reset() {
		score.val = 0;
		death.val = 0;
	}


}
