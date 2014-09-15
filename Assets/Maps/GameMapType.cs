using Assets.Database;
using UnityEngine;
using System.Collections;

public class GameMapType : MonoBehaviour, IDatabaseTypeComponent<Scene>
{
    public Scene type;
    public Scene Type() { return type; }
}
