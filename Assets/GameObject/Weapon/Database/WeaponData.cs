using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class WeaponData : MonoBehaviour
{
	public Weapon weaponPrf;
	public GameObject weaponEquipPrf;

    public int dropRate = 1;

	public Sprite hudBangIcon;

    public GameObject hudAmmoEquipPrf;
    public List<HUDAmmoData> hudAmmoDatas;
}

