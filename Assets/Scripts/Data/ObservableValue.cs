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
		    {
                isDirty = true;
		        postChanged(this);
                isDirty = false;
            }
		}
	}

    public T old { get; private set; }

    public bool isDirty { get; private set; }

    [NonSerialized]
    public Action<ObservableValue<T>> postChanged;

    public ObservableValue() { } 

    public ObservableValue(T _init) { m_Value = _init; }

	public static implicit operator T(ObservableValue<T> _value) {
		return _value.val;
	}
}