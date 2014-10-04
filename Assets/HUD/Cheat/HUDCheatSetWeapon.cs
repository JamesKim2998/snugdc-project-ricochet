using UnityEngine;
using System.Collections;

public class HUDCheatSetWeapon : MonoBehaviour
{

    public UIPopupList popupList;


	void Start () {
	    for (var _weapon = WeaponType.BEGIN + 1; _weapon != WeaponType.END; ++_weapon)
            popupList.items.Add(_weapon.ToString());
	}

    public void OnWeaponSelected()
    {
        if (! Game.Character.character) return;

        WeaponType _selected;
        if (EnumHelper.TryParse(popupList.value, out _selected))
            Game.Character.character.SetWeapon(_selected);
        else
            Debug.LogWarning("Weapon " + popupList.value + " does not exist!");
    }
}
