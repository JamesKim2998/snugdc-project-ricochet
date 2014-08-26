using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterSkinDatabase : MonoBehaviour
{
	private Dictionary<CharacterType, CharacterSkin> m_Skins = new Dictionary<CharacterType, CharacterSkin>();
	public Dictionary<CharacterType, CharacterSkin> skins { get { return m_Skins; } }

	public List<GameObject> editorSkinPrfs;

	public CharacterSkin this[CharacterType type] { get { return skins[type]; } }

	void Start()
	{
		foreach (var _skinPrf in editorSkinPrfs)
		{
			var _type = _skinPrf.GetComponent<CharacterTypeComponent>();
			if (! _type) 
			{
				Debug.LogError("Type is not found!");
				continue;
			}

			var _skin = _skinPrf.GetComponent<CharacterSkin>();
			if (! _skin)
			{
				Debug.LogError("Skin is not found!");
				continue;
			}

			m_Skins.Add(_type.type, _skin);
		}
	}
}

