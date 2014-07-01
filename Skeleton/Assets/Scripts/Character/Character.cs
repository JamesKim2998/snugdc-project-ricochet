using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour {

	private GameObject m_Weapon;
	public string weapon {
		set {
			if (m_Weapon != null) GameObject.Destroy(m_Weapon);
			
			if (value == null) 
			{
				m_Animator.SetTrigger("unequip");
				return;
			}

			m_Weapon = GameObject.Instantiate(Resources.Load(value)) as GameObject;
			m_Weapon.transform.parent = weaponPivot.transform;
			m_Weapon.transform.localPosition = Vector3.zero;
			m_Weapon.transform.localEulerAngles = Vector3.zero;

			var _aimTemp = aim;
			m_Aim = -1; // invalidate aim
			aim = _aimTemp;

			m_Animator.SetTrigger("equip_" + value);
		}
	}

	public WeaponPivot weaponPivot;

	private Animator m_Animator;

	private float m_Aim = 90;
	public float aim { 
		get { return m_Aim; }
		set { 
			if (m_Aim == value) return;

			m_Aim = value;
			
			var _weaponAngle = weaponPivot.transform.eulerAngles;
			_weaponAngle.z = m_Aim - 90;
			weaponPivot.transform.eulerAngles = _weaponAngle;

			m_Animator.SetFloat("aim", m_Aim);
		}
	}

	void Start () {
		m_Animator = GetComponent<Animator>();
	}

	void Update() 
	{
		var _rotation = transform.rotation;

		if (rigidbody2D.velocity.x > 0.3f) 
		{
			_rotation.y = 0;
		}
		else if (rigidbody2D.velocity.x < -0.3f)
		{
			_rotation.y = 180;
		}

		transform.rotation = _rotation;

		m_Animator.SetFloat("speed_x", Mathf.Abs(rigidbody2D.velocity.x));
		m_Animator.SetFloat("velocity_y", rigidbody2D.velocity.y);
	}

	public void Unequip()
	{
		weapon = null;
	}
}
