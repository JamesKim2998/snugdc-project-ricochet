using System;
using UnityEngine;
using System.Collections;

public class Updatable : MonoBehaviour
{

    public Action<Updatable> postUpdate;

	void Update ()
	{
	    if (postUpdate != null) postUpdate(this);
	}
}
