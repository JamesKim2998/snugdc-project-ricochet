using System;
using UnityEngine;
using System.Collections;

public class DestroySelf : MonoBehaviour
{
    public bool applyOnStart = false;
    public float delay = -1f;

    void Start()
    {
        if (applyOnStart)
            Execute();
    }

    [Obsolete]
	public void Destroy() 
	{
		Destroy (gameObject);
	}

    public void Execute()
    {
        if (delay <= 0)
            Destroy(gameObject);
        else 
            Destroy(gameObject, delay);
    }
}
