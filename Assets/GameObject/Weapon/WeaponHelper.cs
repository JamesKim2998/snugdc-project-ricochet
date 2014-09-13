using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class WeaponHelper
{
	public static WeaponType Convert(int _type) {
		var _weaponType = (WeaponType) _type;
		if (_weaponType >= WeaponType.BEGIN && _weaponType <= WeaponType.END) {
			return _weaponType;
		} else {
			Debug.LogError("Weapon " + _type + " does not exist. Ignore.");
			return WeaponType.NONE;
		}
	}

	private static readonly Dictionary<WeaponAnimationGroup, string> m_Triggers = new Dictionary<WeaponAnimationGroup, string> {
		{ WeaponAnimationGroup.MACHINE_GUN, "machine_gun" },
	};
	
	public static string GetTrigger(WeaponAnimationGroup _animationGroup) {
		return m_Triggers[_animationGroup];
	}
}

