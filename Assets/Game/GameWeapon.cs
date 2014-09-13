using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameWeapon 
{
    public List<WeaponType> weaponSet = new List<WeaponType>();

	public int count { get { return weaponSet.Count; }}
	
	public void Purge()
	{
        weaponSet.Clear();
	}

	public WeaponType Random() 
	{
		return weaponSet[UnityEngine.Random.Range(0, count)];
	}

}
