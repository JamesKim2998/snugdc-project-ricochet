using UnityEngine;
using System.Collections;

public class DestroyTarget : MonoBehaviour
{
	public GameObject target;
	public void Execute()
	{
		if (target) 
		{
			Destroy(target);
		}
		else 
		{
			Debug.LogWarning("Target is not specified! Ignore.");
		}
	}
}

