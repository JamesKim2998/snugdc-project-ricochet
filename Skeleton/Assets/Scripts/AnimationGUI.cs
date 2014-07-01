using UnityEngine;
using System.Collections;

public class AnimationGUI : MonoBehaviour {

	public Character character;
	private Animator m_Animator;

	private float m_Aim = 90f;
	private float m_Speed = 0f;

	void Start()
	{
		m_Animator = character.GetComponent<Animator>();
	}

	void FixedUpdate()
	{
		var _velocity = character.rigidbody2D.velocity;
		_velocity.x = Mathf.Lerp(_velocity.x, m_Speed, 0.2f);
		character.rigidbody2D.velocity = _velocity;
	}

	void OnGUI()
	{
		GUILayout.BeginHorizontal();

		if (GUILayout.Button("crouch"))
		{
			m_Animator.SetTrigger("crouch_lower");
		}

		if (GUILayout.Button("stand"))
		{
			m_Animator.SetTrigger("stand_lower");
		}

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
		{
			character.weapon = null;
		}
		
		string[] _weapons = {
			"simple_gun", "uzi_gun", "grenade", 
		};
		
		GUILayout.BeginHorizontal();

		foreach (var _weapon in _weapons)
		{
			if (GUILayout.Button(_weapon))
				character.weapon = _weapon;
		}
			
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label(string.Format("aim   {0}", (int) character.aim), GUILayout.Width(100f));
		m_Aim = GUILayout.HorizontalSlider(m_Aim, 0, 180);
		character.aim = m_Aim;
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label(string.Format("speed {0}", (int) m_Speed), GUILayout.Width(100f));
		m_Speed = GUILayout.HorizontalSlider(m_Speed, -8, 8);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();

		if (GUILayout.Button("shoot"))
		{
			m_Animator.SetTrigger("shoot");
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
