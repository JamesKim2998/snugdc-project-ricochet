using System;
using UnityEngine;
using System.Collections;

public class CharacterTypeComponent : MonoBehaviour, IDatabaseTypeComponent<CharacterType>
{
    public CharacterType type;
	public CharacterType Type() { return type; }
}

