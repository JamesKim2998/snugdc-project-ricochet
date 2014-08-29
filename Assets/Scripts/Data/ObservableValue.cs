using System;

public class ObserverableValue<T>
{
	private T m_Value;
	public T Get() { return m_Value; } 
	public void Set(T _value) { 
		var _old = m_Value; 
		m_Value = _value; 
		if (postChanged != null) 
			postChanged(m_Value, _old); 
	}

	public Action<T, T> postChanged;
}

