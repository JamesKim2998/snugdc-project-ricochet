using UnityEngine;
using System.Collections;

public interface GameModePropertyTimeLimit
{
    int timeLimit { get; }
    int timeLeft { get; }
}
