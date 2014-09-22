using UnityEngine;
using System;
using System.Collections;

public class ObservableValue<T> where T : System.IEquatable<T> {
	private T m_Value;

    public T val { 
		get { return m_Value; } 
		
		set {
			if (m_Value.Equals( value) ) return;
			old = m_Value;
			m_Value = value;
			if (postChanged != null) 
				postChanged(this);
		}
	}

    public T old { get; private set; }

    public event Action<ObservableValue<T>> postChanged;
	
	public static implicit operator T(ObservableValue<T> _statistic) {
		return _statistic.val;
	}
}