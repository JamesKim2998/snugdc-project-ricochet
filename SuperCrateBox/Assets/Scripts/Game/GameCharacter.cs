using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class GameCharacter 
{
	public Game game { get { return m_Game; } set { m_Game = value; } }
	private Game m_Game;

	public float maxUpForce = 2f;
	public float upForce = 30f;
	private float m_UpForceLeft = 0;

	public GameObject weaponDefault;

	public delegate void PostCharacterChanged(Character _character);
	public event PostCharacterChanged postCharacterChanged;

	private Character m_Character;
	public Character characterExpose;
	public Character character {
		get { return m_Character; }
		set { 
			if (character != null) 
			{
				if (value != null)
					Debug.Log("trying to set character, but there's already a character!");

				game.camera_.Unbind(GetHashCode());
				character.GetComponent<Destroyable>().postDestroy -= ListenDestroy;
			}

			m_Character = value;

			if (m_Character != null) 
			{
				m_Character.renderers.SetColor(characterColor);

				game.camera_.Bind(GetHashCode(), m_Character.transform);

				if (weaponDefault != null)
				{
					var _weapon = GameObject.Instantiate(weaponDefault) as GameObject;	
					m_Character.weapon = _weapon.GetComponent<Weapon>();
				}

				m_Character.GetComponent<Destroyable>().postDestroy += ListenDestroy;
			}

			if (postCharacterChanged != null) 
				postCharacterChanged(m_Character);
		}
	}

	public List<Color> characterColorPresets;
	[HideInInspector]
	public Color characterColor;

	public void Start()
	{
		if (characterColorPresets.Count == 0) 
		{
			characterColor = Color.white;
		}
		else 
		{
			characterColor = GenericHelper.SelectRandom (characterColorPresets);
		}

		if (characterExpose)
			character = characterExpose;
	}

	public void Update () 
	{
		if (character == null) return;

		var _vertical = Input.GetAxis("Vertical");
		if (Mathf.Abs(_vertical) > 0.05f)
			character.ChangeAim(_vertical);

		if (Input.GetButtonDown("Jump")) 
		{
			if (character.jumpable) 
			{
				m_UpForceLeft = maxUpForce;
				character.Jump();
			}
		}

		if (Input.GetButtonDown("Crouch"))
		{
			if (character.isCrouching) 
			{
				character.Stand();
			}
			else 
			{
				character.Crouch();
			}
		}

		if (Input.GetButtonDown("Fire1")) 
		{
			if (character.shootable) 
				character.Shoot();
		}
		else if (Input.GetButtonUp("Fire1")) 
		{
			if (character.weapon != null)
				(character.weapon as SimpleWeapon).Rest();
		}


		if (Input.mousePresent)
		{
			var _mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			var _mousePositionDelta = _mousePosition - character.crossHair.transform.position;
			character.direction = _mousePositionDelta.x > 0 ? 1 : -1;

			var _mouseAngle = TransformHelper.VectorToDeg(_mousePositionDelta) + 90;

			if (character.direction < 0) 
			{
				_mouseAngle = 360 - _mouseAngle;
			}

			_mouseAngle = MathHelper.Mod(_mouseAngle, 360f);

			var _aimDelta = _mouseAngle - character.aim;
			character.aim += Mathf.Clamp(_aimDelta, -character.aimSpeed, character.aimSpeed);
		}
	}

	public void FixedUpdate() 
	{
		if (character == null) return;

		if (character.movable) 
		{
			float _horizontal = Input.GetAxis("Horizontal");
			if (! _horizontal.Equals(0)) 
				character.Move(_horizontal);
			
			float _vertical = Input.GetAxis("Vertical");
			
			if (character.floating && _vertical > 0 && m_UpForceLeft > 0) 
			{
				m_UpForceLeft -= upForce * Time.fixedDeltaTime;
				m_Character.rigidbody2D.AddForce(new Vector2(0, upForce));
			}
		}
		
	}

	public void ListenDestroy(Destroyable _destroyable) 
	{
		character = null;
	}
}
