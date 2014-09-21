using System;
using UnityEngine;
using System.Collections;

public class DestroyableObserver<T> : MonoBehaviour where T: Component
{
    private Destroyable m_Target;

    public T target
    {
        get { return m_Target != null ? m_Target.GetComponent<T>() : null; }
        set
        {
            if (target == value)
                return;

            if (m_Target)
                m_Target.postDestroy -= ListenDestroy;

            var _old = target;
            m_Target = value ? value.GetComponent<Destroyable>() : null;

            if (m_Target)
            {
                m_Target.postDestroy += ListenDestroy;
                enabled = true;
            }
            else
            {
                enabled = false;
            }

            if (postTargetChanged != null) postTargetChanged(target, _old);
        }
    }

    public Action<T, T> postTargetChanged;
    public Action<T> postTargetDestroyed;

    void Awake()
    {
        enabled = false;
    }

    void OnDestroy()
    {
        target = null;
    }

	void Update ()
	{
	    if (!target) return;
	    transform.localPosition = target.transform.localPosition;
	}

    void ListenDestroy(Destroyable _destroyable)
    {
        postTargetDestroyed(target);
        target = null;
    }
}
