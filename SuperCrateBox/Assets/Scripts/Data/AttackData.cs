using UnityEngine;
using System.Collections;

[System.Serializable]
public class AttackData {

	public Vector2 velocity;
	public int damage;
	public Character owner;

	public static implicit operator int(AttackData _attackData) {
		return _attackData.damage;
	}
}
