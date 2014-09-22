using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class ObservableValue<T> {
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

    [NonSerialized]
    public Action<ObservableValue<T>> postChanged;
	
	public static implicit operator T(ObservableValue<T> _value) {
		return _value.val;
	}
}