using UnityEngine;
using System.Collections;

public class Database : Singleton<Database> 
{
	public CharacterSkinDatabase skin;
	public static CharacterSkinDatabase Skin { get { return Instance.skin; } }

	public WeaponDatabase weapon;
	public static WeaponDatabase Weapon { get { return Instance.weapon; }}

	public void Apply(DatabaseDef _def)
	{
		skin = (Instantiate(_def.skinPrf.gameObject) as GameObject).GetComponent<CharacterSkinDatabase>();
		skin.transform.parent = gameObject.transform;

		weapon = (Instantiate(_def.weaponPrf.gameObject) as GameObject).GetComponent<WeaponDatabase>();
		weapon.transform.parent = gameObject.transform;
	}
}
