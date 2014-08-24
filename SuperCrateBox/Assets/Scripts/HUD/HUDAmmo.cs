using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDAmmo : MonoBehaviour 
{
	public GameObject ammoPrf;

	public Vector2 offset = Vector2.zero;
	public Vector2 interval = Vector2.zero;

	private List<GameObject> m_Ammos;

	private int m_Ammo = 0;
	public int ammo {
		get { return m_Ammo; }
		set { 
			for (int _i = m_Ammo; _i < value; ++_i)
			{
				Vector2 _position = offset + _i * interval;
				_position += new Vector2(transform.position.x, transform.position.y);
				var _bullet = GameObject.Instantiate(ammoPrf, _position, Quaternion.identity) as GameObject;
				_bullet.transform.parent = transform;
				m_Ammos.Add(_bullet);
			}
			
			for (int _i = value; _i < m_Ammo; ++_i)
			{
				GameObject.Destroy(m_Ammos[m_Ammos.Count - 1]);
				m_Ammos.RemoveAt(m_Ammos.Count - 1);
			}

			m_Ammo = value;
		}
	}

	private int m_AmmoMax = 0;
	public int ammoMax {
		get { return m_AmmoMax; }
		set { m_AmmoMax = value; }
	}
	
	public Weapon targetWeapon { 
		get { return m_TargetWeapon; } 
		set {
			if (m_TargetWeapon == value)
				return;

			if (m_TargetWeapon)
				m_TargetWeapon.postShoot -= ListenShoot;

			m_TargetWeapon = value;

			if (m_TargetWeapon)
			{
				ammo = m_TargetWeapon.ammo;
				ammoMax = m_TargetWeapon.ammoMax;
				m_TargetWeapon.postShoot += ListenShoot;
			}
			else
			{
				ammo = 0;
				ammoMax = 0;
			}
		}
	}
	
	private Weapon m_TargetWeapon;

	void Start()
	{
		m_Ammos = new List<GameObject> ();
		Game.Character().postCharacterChanged += ListenCharacterChanged;
	}

	void OnDestroy()
	{
		if (targetWeapon) targetWeapon = null;
		Game.Character().postCharacterChanged -= ListenCharacterChanged;
	}

	void ListenCharacterChanged(Character _character)
	{
		if (_character == null) return;
		_character.postWeaponChanged += ListenWeaponChanged;
	}

	void ListenWeaponChanged(Character _character, Weapon _old)
	{
		targetWeapon = _character.weapon;
	}

	void ListenShoot(Weapon _weapon, Projectile _projectile)
	{
		ammo = _weapon.ammo;
		ammoMax = _weapon.ammoMax;
	}
}