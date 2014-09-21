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
		
		if (! m_WeaponEquip)
		{
			Debug.LogError("Trying to throw away, but weapon equip does not exist in the first place.");
			return;
		}
		
        ThrowAway();
        Unequip();
	}

}

