using UnityEngine;
using System.Collections;

public class Activator : MonoBehaviour 
{
	public GameObject target;

	public void Activate()
	{
		target.SetActive (true);
	}

	public void Deactivate()
	{
		target.SetActive (false);
	}
}
