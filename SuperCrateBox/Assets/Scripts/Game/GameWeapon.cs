using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameWeapon 
{
	public List<string> weaponSet = new List<string>();

	public int count { get { return weaponSet.Count; }}
	
	public void Purge()
	{
		weaponSet = new List<string> ();
	}

	public string Random() 
	{
		return weaponSet[UnityEngine.Random.Range(0, count)];
	}

}
