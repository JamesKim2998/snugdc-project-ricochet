using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameCharacter {

	public float maxUpForce = 2f;
	public float upForce = 30f;
	private float m_UpForceLeft = 0;

	public delegate void PostCharacterChanged(Character _character);
	public event PostCharacterChanged postCharacterChanged;

	private Character m_Character;
	public Character character {
		get { return m_Character; }
		set { 
			if (character != null) {
				if (value != null) {
					Debug.Log("trying to set character, but there's already a character!");
				}

				character.GetComponent<Destroyable>().postDestroy -= ListenDestroy;
			}

			m_Character = value;

			if (character != null) {
				character.GetComponent<Destroyable>().postDestroy += ListenDestroy;
			}

			if (postCharacterChanged != null) {
				postCharacterChanged(m_Character);
			}
		}
	}

	public void Update () {
		if (character == null) return;

		if (Input.GetButtonDown("Jump")) {
			if (character.jumpable) {
				m_UpForceLeft = maxUpForce;
				character.Jump();
			}
		}

		if (Input.GetButtonDown("Fire1")) {
			if (character.shootable) 
				character.Shoot();
		}
	}

	public void FixedUpdate() {

		if (character == null) return;

		if (character.movable) {
			
			float _horizontal = Input.GetAxis("Horizontal");
			if (! _horizontal.Equals(0)) character.Move(_horizontal);
			
			float _vertical = Input.GetAxis("Vertical");
			
			if (character.floating && _vertical > 0 && m_UpForceLeft > 0) {
				m_UpForceLeft -= upForce * Time.fixedDeltaTime;
				m_Character.rigidbody2D.AddForce(new Vector2(0, upForce));
			}
		}
		
	}

	public void ListenDestroy(Destroyable _destroyable) {
		character = null;
	}
}
