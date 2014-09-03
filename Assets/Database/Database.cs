using UnityEngine;
using System.Collections;

public class Database : Singleton<Database> 
{
	public CharacterSkinDatabase skin;
	public static CharacterSkinDatabase Skin { get { return Instance.skin; } }

	public WeaponDatabase weapon;
	public static WeaponDatabase Weapon { get { return Instance.weapon; }}
	
	public ProjectileDatabase projectile;
	public static ProjectileDatabase Projectile { get { return Instance.projectile; } }

	public void Apply(DatabaseDef _def)
	{
		skin = ((GameObject) Instantiate(_def.skinPrf.gameObject)).GetComponent<CharacterSkinDatabase>();
		skin.transform.parent = gameObject.transform;

		weapon = ((GameObject) Instantiate(_def.weaponPrf.gameObject)).GetComponent<WeaponDatabase>();
		weapon.transform.parent = gameObject.transform;

		projectile = ((GameObject) Instantiate(_def.projectilePrf.gameObject)).GetComponent<ProjectileDatabase>();
		projectile.transform.parent = gameObject.transform;
	}
}
