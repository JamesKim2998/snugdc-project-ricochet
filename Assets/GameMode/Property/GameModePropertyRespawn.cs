using UnityEngine;
using System.Collections;

public interface GameModePropertyRespawn
{
    int respawnLeft { get; }
    int respawnLimit { get; }
    int respawnDelay { get; }
}
