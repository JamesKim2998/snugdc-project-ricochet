using UnityEngine;
using System.Collections;

[System.Serializable]
public class AttackData 
{
	public static readonly AttackData DAMAGE_MAX = new AttackData(1000);

	public AttackData(int _damage)
	{
		damage = _damage;
	}

	public string owner;
	public Vector2 velocity = Vector2.zero;
	public int damage = 0;

	public static implicit operator int(AttackData _attackData) {
		return _attackData.damage;
	}
}
