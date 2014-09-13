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

	    var _position = weaponEquip.transform.localPosition;
	    _position.x += -0.5f;
	    weaponEquip.transform.localPosition = _position;

        weaponEquip.rigidbody2D.velocity += -3 * new Vector2 { x = transform.right.x, y = transform.right.y, };
        weaponEquip.rigidbody2D.angularVelocity += transform.right.x > 0 ? 100 : -100;

        Destroy(weaponEquip.gameObject, 2);

		Unequip();
	}

}

