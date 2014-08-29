using UnityEngine;
using System.Collections;

public class UICharacterSelectorTest : MonoBehaviour
{
	public UICharacterSelector selector;
	public CharacterType type;

	void Start ()
	{
		Invoke("Execute", 0.5f);
	}

	void Execute()
	{
		selector.player = Network.player.guid;
		selector.type = type;
	}
}

