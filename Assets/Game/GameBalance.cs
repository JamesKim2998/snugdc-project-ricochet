using UnityEngine;
using System.Collections;

public class GameBalance 
{
    public GameBalanceScore score = new GameBalanceScore();

    public void Apply(GameBalanceDef _def)
    {
        score.kill = _def.score.kill;
        score.weaponPickUp = _def.score.weaponPickup;
    }
}
