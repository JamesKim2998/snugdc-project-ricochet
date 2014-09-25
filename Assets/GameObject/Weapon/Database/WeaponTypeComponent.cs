using UnityEngine;
using System.Collections;

public class WeaponTypeComponent : MonoBehaviour, IDatabaseTypeComponent<WeaponType>
{
	public WeaponType type;

    public WeaponType Type()
    {
        return type;
    }
}

