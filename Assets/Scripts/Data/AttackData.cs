using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;

[System.Serializable]
public class AttackData 
{
	public static readonly AttackData DAMAGE_MAX = new AttackData(1000);

	public AttackData(int _damage)
	{
		damage = _damage;
	}

	public string ownerPlayer;

    public WeaponType weapon = WeaponType.NONE;
    public ProjectileType projectile = ProjectileType.NONE;

	[System.NonSerialized]
	public Vector2 velocity = Vector2.zero;
	public int damage = 0;
    
	public static implicit operator int(AttackData _attackData) 
	{
		return _attackData.damage;
	}
	
	public string Serialize() 
	{
		return NetworkSerializer.Serialize(this)
	        + Serializer.serialize(velocity);
	}

	public static AttackData Deserialize(string _serial)
	{
	    Vector2 _velocity;
        _serial = _serial.Substring(0, Serializer.deserialize(_serial, out _velocity));

        AttackData _attackData;
		NetworkSerializer.Deserialize(_serial, out _attackData);

		_attackData.velocity = _velocity;
		return _attackData;
	}
}
