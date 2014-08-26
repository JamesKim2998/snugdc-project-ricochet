using UnityEngine;
using System.Collections;

public class CharacterSkinTest : MonoBehaviour
{
	public CharacterRenderer renderer_;
	public CharacterSkin skin;

	void Start ()
	{
		skin.Apply(renderer_);
	}
}

