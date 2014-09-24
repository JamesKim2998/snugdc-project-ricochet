using UnityEngine;
using System.Collections;

public class WeaponDecoratorRandomScatter : MonoBehaviour
{
    public float deviation;

    void Start()
    {
        SimpleRNG.GetNormal(0, deviation);
	}
	
	void Update () {
	
	}
}
