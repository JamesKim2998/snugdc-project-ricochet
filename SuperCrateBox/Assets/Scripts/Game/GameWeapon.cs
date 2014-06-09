using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameWeapon {

	public GameObject[] weapons;

	public int count { get { return weapons.Length; }}

	public GameObject Random() {
		return weapons[UnityEngine.Random.Range(0, count)];
	}
}
