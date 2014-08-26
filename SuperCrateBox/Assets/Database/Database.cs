using UnityEngine;
using System.Collections;

public class Database : Singleton<Database> 
{
	public CharacterSkinDatabase skin;
	public static CharacterSkinDatabase Skin { get { return Instance.skin; } }

	public void Apply(DatabaseDef _def)
	{
		skin = (Instantiate(_def.skinPrf.gameObject) as GameObject).GetComponent<CharacterSkinDatabase>();
		skin.transform.parent = gameObject.transform;
	}
}
