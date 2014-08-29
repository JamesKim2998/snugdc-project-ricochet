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

	public string owner;

	[System.NonSerialized]
	public Vector2 velocity = Vector2.zero;
	public int damage = 0;

	public static implicit operator int(AttackData _attackData) 
	{
		return _attackData.damage;
	}
	
	public string Serialize() 
	{
		var _serial = NetworkSerializer.Serialize(this);
		_serial += ',';
		_serial += velocity.x;
		_serial += ',';
		_serial += velocity.y;
		return _serial;
	}

	public static AttackData Deserialize(string _serial)
	{
		AttackData _attackData;
		var _velocityYIdx = _serial.LastIndexOf(',');
		var _velocityY = float.Parse(_serial.Substring(_velocityYIdx + 1));
		_serial = _serial.Substring(0, _velocityYIdx);

		var _velocityXIdx = _serial.LastIndexOf(',');
		var _velocityX = float.Parse(_serial.Substring(_velocityXIdx + 1));
		_serial = _serial.Substring(0, _velocityXIdx);

		NetworkSerializer.Deserialize(_serial, out _attackData);
		_attackData.velocity = new Vector2(_velocityX, _velocityY);

		return _attackData;
	}
}
