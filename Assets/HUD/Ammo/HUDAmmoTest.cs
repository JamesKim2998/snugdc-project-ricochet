using UnityEngine;
using System.Collections;

public class HUDAmmoTest : MonoBehaviour
{
    public HUDAmmo ammo;
    public Weapon weapon;

    void Start()
    {
        ammo.weapon = weapon;
    }
}
