using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;

public class SetCounter<T> where T : System.IEquatable<T>
{
    private readonly HashSet<T> m_Counter = new HashSet<T>();

    public int val
    {
        get { return m_Counter.Count; }
    }

    public int old { get; private set; }
    public int delta { get { return val - old; } }

    public bool Add(T _value)
    {
        if (! m_Counter.Add(_value))
        {
            Debug.Log(_value + " does already exist! Ignore.");
            return false;
        }

        old = val - 1;

        if (postAdd != null) postAdd(this, _value);
        if (postChanged != null) postChanged(this, _value);
        return true;
    }

    public bool Remove(T _value)
    {
        if (! m_Counter.Remove(_value))
        {
            Debug.Log(_value + " does not exist! Ignore.");
            return false;
        }

        old = val + 1;

        if (postRemove != null) postRemove(this, _value);
        if (postChanged != null) postChanged(this, _value);
        return true;
    }

    public void Clear()
    {
        old = val;
        m_Counter.Clear();
        if (postChanged != null) postChanged(this, default(T));
    }

    public Action<SetCounter<T>, T> postAdd;
    public Action<SetCounter<T>, T> postRemove;
    public Action<SetCounter<T>, T> postChanged;

    public SetCounter()
    {
        old = 0;
    }

    public static implicit operator int(SetCounter<T> _counter)
    {
        return _counter.val;
    }
}