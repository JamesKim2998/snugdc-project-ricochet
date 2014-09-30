using System.ComponentModel;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameWeapon
{
    private List<WeaponType> m_WeaponSet = new List<WeaponType>();
    public List<WeaponType> weaponSet
    {
        get { return m_WeaponSet; }
        set
        {
            if (m_WeaponSet == value) return;
            m_WeaponSet = value;
            Refresh();;
        }
    }

    private int m_TotalDropRate = 0;

	public int count { get { return weaponSet.Count; }}
	
	public void Purge()
	{
        weaponSet.Clear();
	}

    public void Refresh()
    {
        m_TotalDropRate = 0;

        foreach (var _weaponType in weaponSet)
        {
            var _weaponData = Database.Weapon[_weaponType];
            if (_weaponData)
                m_TotalDropRate += _weaponData.dropRate;
        }
    }

	public WeaponType Random()
	{
	    if (m_TotalDropRate == 0 && weaponSet.Count > 0)
	        Refresh();

	    var _drop = UnityEngine.Random.Range(0, m_TotalDropRate);
	    var _weapon = WeaponType.NONE;

	    foreach (var _weaponType in weaponSet)
        {
            var _weaponData = Database.Weapon[_weaponType];
            if (_weaponData)
            {
                _drop -= _weaponData.dropRate;
                if (_drop < 0)
                {
                    _weapon = _weaponType;
                    break;
                }
            }
	    }

        return _weapon;
	}

}
