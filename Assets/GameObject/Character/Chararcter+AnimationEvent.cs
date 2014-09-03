using UnityEngine;
using System.Collections;

public partial class Character : MonoBehaviour {
	
	void ListenAnimationEventThrowAway()
	{
		if (! weapon)
		{
			Debug.LogError("Trying to throw away, but weapon is not equipped in the first place.");
			return;
		}
		
		if (! weaponEquip)
		{
			Debug.LogError("Trying to throw away, but weapon equip does not exist in the first place.");
			return;
		}
		
		weaponEquip.physicsEnabled = true;
		weaponEquip.transform.parent = null;
		weaponEquip.transform.localScale = Vector3.one;

		Unequip();
	}

}

