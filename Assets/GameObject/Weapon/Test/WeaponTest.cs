using UnityEngine;
using System.Collections;

public class WeaponTest : MonoBehaviour
{
    public Weapon weapon;

    public void Shoot()
    {
        if (weapon.IsShootable())
            weapon.Shoot();
    }
}
