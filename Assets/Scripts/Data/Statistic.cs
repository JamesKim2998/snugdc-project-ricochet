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
			if (m_Value.Equals( value) ) return;
			m_OldValue = m_Value;
			m_Value = value;
			if (postChanged != null) 
				postChanged(this);
		}
	}
	
	public T old { get { return m_OldValue; }}
	
	public event Action<Statistic<T>> postChanged;
	
	public static implicit operator T(Statistic<T> _statistic) {
		return _statistic.val;
	}
}