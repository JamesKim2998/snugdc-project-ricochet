using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class CharacterGameTest : MonoBehaviour
{
    public List<Character> characters;

    void Start()
    {
        foreach (var _character in characters)
        {
            _character.ownerPlayer = Network.player.guid;

            _character.postDead += (Character _theCharacter) =>
            {
                characters.Remove(_theCharacter);
                Game.Character.GameCharacter_OnDead(
                    Network.player.guid,
                    _theCharacter.id,
                    _theCharacter.lastAttackData.Serialize());
            };

            Game.Character.Add(_character);
        }
    }

    public void Kill()
    {
        characters.First().Hit(new AttackData(1000));
    }
}
