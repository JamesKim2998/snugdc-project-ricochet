using UnityEngine;
using System.Collections;

public class AnimationGUI : MonoBehaviour {

	public Character character;
	private Animator m_Animator;

	void Start()
	{
		m_Animator = character.GetComponent<Animator>();
	}

	void OnGUI()
	{
		GUILayout.BeginHorizontal();
		
		if (GUILayout.Button("stand"))
			character.Stand();

		if (GUILayout.Button("crouch"))
			character.Crouch();

		if (GUILayout.Button("jump"))
		{
			var _velocity = character.rigidbody2D.velocity;
			_velocity.y += 5f;
			character.rigidbody2D.velocity = _velocity;

			m_Animator.SetTrigger("jump_upper");
			m_Animator.SetTrigger("jump_lower");
		}

		GUILayout.EndHorizontal();

		if (GUILayout.Button("unequip")) 
			character.Unequip();
		
		string[] _weaponNames = {
			"simple_gun", "uzi_gun", "grenade", 
			"laser_gun", "rocket_launcher", "mine_weapon",  
		};

		int _weaponCnt = 0;
		GUILayout.BeginHorizontal();

		foreach (var _weaponName in _weaponNames)
		{
			if (_weaponCnt++ % 3 == 0) 
			{
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
			}

			if (GUILayout.Button(_weaponName)) 
			{
				var _weapon = GameObject.Instantiate(Resources.Load(_weaponName)) as GameObject;

				if (_weapon == null) 
				{
					Debug.LogError("Weapon script is not found in weapon!");
				}
				else 
				{
					character.weapon = _weapon.GetComponent<Weapon>();
				}
			}
		}
			
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();

		if (GUILayout.Button("shoot"))
		{
			if (character.shootable)
				character.Shoot();
		}
		
		if (GUILayout.Button("rest"))
		{
			m_Animator.SetTrigger("rest");
		}

		if (GUILayout.Button("throw_away"))
		{
			m_Animator.SetTrigger("throw_away");
		}

		GUILayout.EndHorizontal();
	}
}
