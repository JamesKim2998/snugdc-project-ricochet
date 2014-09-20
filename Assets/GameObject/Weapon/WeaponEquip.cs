using UnityEngine;
using System.Collections;

public class WeaponEquip : MonoBehaviour
{
	public GameObject body;

	private bool m_PhysicsEnabled = false;
	public bool physicsEnabled { 
		get { return m_PhysicsEnabled; }
		set {
			if (m_PhysicsEnabled == value)
				return;

		    m_PhysicsEnabled = value;

		    if (value)
		    {
		        gameObject.AddComponent<Rigidbody2D>();
		        rigidbody2D.angularDrag = 0.3f;
		    }
		    else
		    {
				Destroy(gameObject.rigidbody2D);
		    }

			body.SetActive(value);
		}
	}
}

