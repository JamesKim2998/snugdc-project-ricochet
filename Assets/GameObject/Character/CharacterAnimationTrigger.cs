using UnityEngine;
using System.Collections;

public static class CharacterAnimationTrigger
{
	public static string JUMP_LOWER = "jump_lower";
	public static string JUMP_UPPER = "jump_upper";
	public static string UNEQUIP = "unequip";

	public static string ArmWeaponEquip(string _animationGroup) { return "arm_equip_" + _animationGroup; }
	public static string UpperWeaponEquip(string _animationGroup) { return "upper_equip_" + _animationGroup; }
}

