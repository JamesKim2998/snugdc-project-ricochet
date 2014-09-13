using UnityEngine;
using System.Collections;

public class HUDBangEntity : MonoBehaviour
{
    public UI2DSprite killerIcon;
    public UILabel killerName;

    public UI2DSprite victimIcon;
    public UILabel victimName;

    public UI2DSprite weapon;

    public void Refresh(Character _victim)
    {
        var _attackData = _victim.lastAttackData;

        var _killerInfo = Global.Player()[_attackData.ownerPlayer];
        killerIcon.sprite2D = Database.Skin[_killerInfo.characterSelected].head.sprite;
        killerName.text = _killerInfo.name;

        var _victimInfo = Global.Player()[_victim.ownerPlayer];
        victimIcon.sprite2D = Database.Skin[_victimInfo.characterSelected].head.sprite;
        victimName.text = _victimInfo.name;

        if (_attackData.weapon != WeaponType.NONE)
        {
            weapon.sprite2D = Database.Weapon[_attackData.weapon].bangIcon;
        }
        else
        {
            Debug.LogError("Weapon type is not specified!");
        }
    }
}
