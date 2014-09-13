using UnityEngine;
using System.Collections;

public class HUDBangEntityTest : MonoBehaviour
{
    public HUDBangEntity bangEntity;
    public Character victim;
    

	void Start ()
	{
	    victim.ownerPlayer = Network.player.guid;

        var _attackData = new AttackData(100)
        {
            ownerPlayer = Network.player.guid, 
            projectile = ProjectileType.RED_BULLET,
            weapon = WeaponType.ASSAULT_RIFLE,
        };

        victim.Hit(_attackData);

	    bangEntity.Refresh(victim);
	}
	
}
