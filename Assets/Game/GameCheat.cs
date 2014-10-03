using UnityEngine;
using System.Collections;

public class GameCheat : MonoBehaviour
{
	public void KillCharacter()
	{
        var _character = Game.Character.character;
	    if (! _character) return;
        _character.Hit(AttackData.DAMAGE_MAX);
	}

    private const int INVINCIBLE_FLAG = 12834123;
    private bool _invincibleEnabled = false;
    public void ToggleInvincible()
    {
        var _character = Game.Character.character;
        if (! _character) return;
        if (_invincibleEnabled) _character.hitDisabled -= INVINCIBLE_FLAG;
        else _character.hitDisabled += INVINCIBLE_FLAG;
        _invincibleEnabled = !_invincibleEnabled;
    }

    public void InfiniteAmmo()
    {
        var _character = Game.Character.character;
        if (_character && _character.weapon)
            _character.weapon.ammo = 1000;
    }

    public void OverGame()
    {
        Game.Progress.TryOverGame();
    }

}
