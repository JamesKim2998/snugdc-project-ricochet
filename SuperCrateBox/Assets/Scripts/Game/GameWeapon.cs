using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameWeapon {

	public string[] weapons;

	public int count { get { return weapons.Length; }}

	public string Random() {
		return weapons[UnityEngine.Random.Range(0, count)];
	}
}
