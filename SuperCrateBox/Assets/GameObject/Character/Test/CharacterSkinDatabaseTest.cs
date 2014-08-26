using UnityEngine;
using System.Collections;

public class CharacterSkinDatabaseTest : MonoBehaviour
{
	public CharacterType type;
	public CharacterRenderer renderer_;

	void Start ()
	{
		Invoke("Execute", 0.1f);
	}

	void Execute()
	{
		Debug.Log(type);
		Debug.Log(Database.Skin.skins.Count);
		Database.Skin[type].Apply(renderer_);
	}
}

