using UnityEngine;
using System.Collections;

public class GameCheat : MonoBehaviour
{
	public void KillCharacter()
	{
		Game.Character().character.Hit(AttackData.DAMAGE_MAX);
	}

}

