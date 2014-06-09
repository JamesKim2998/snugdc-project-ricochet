using UnityEngine;
using System.Collections;

[System.Serializable]
public class GamePlayer {

	private Shooter m_Shooter;

	public float maxUpForce = 2.0f;
	public float upForce = 2.0f;
	private float m_UpForceLeft = 0;

	public delegate void PostPlayerChanged(Shooter _shooter);
	public event PostPlayerChanged postPlayerChanged;

	public Shooter shooter {
		get { return m_Shooter; }
		set { 
			if (shooter != null) {
				if (value != null) {
					Debug.Log("trying to set shooter, but there's already a shooter!");
				}

				shooter.GetComponent<Destroyable>().postDestroy -= ListenDestroy;
			}

			m_Shooter = value;

			if (shooter != null) {
				shooter.GetComponent<Destroyable>().postDestroy += ListenDestroy;
			}

			if (postPlayerChanged != null) {
				postPlayerChanged(m_Shooter);
			}
		}
	}

	public void Update () {
		if (shooter == null) return;

		if (Input.GetButtonDown("Jump")) {
			if (shooter.jumpable) {
				m_UpForceLeft = maxUpForce;
				shooter.Jump();
			}
		}

		if (Input.GetButtonDown("Fire1")) {
			if (shooter.shootable) 
				shooter.Shoot();
		}
	}

	public void FixedUpdate() {

		if (shooter == null) return;

		if (shooter.movable) {
			
			float _horizontal = Input.GetAxis("Horizontal");
			if (! _horizontal.Equals(0)) shooter.Move(_horizontal);
			
			float _vertical = Input.GetAxis("Vertical");
			
			if (shooter.floating && _vertical > 0 && m_UpForceLeft > 0) {
				m_UpForceLeft -= upForce * Time.fixedDeltaTime;
				m_Shooter.rigidbody2D.AddForce(new Vector2(0, upForce));
			}
		}
		
	}

	public void ListenDestroy(Destroyable _destroyable) {
		shooter = null;
	}
}
