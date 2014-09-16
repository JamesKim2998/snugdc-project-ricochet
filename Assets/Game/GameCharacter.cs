using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public partial class GameCharacter : MonoBehaviour
{
	public float downForce = 30f;
	public float maxUpForce = 2f;
	public float upForce = 30f;
	private float m_UpForceLeft = 0;

	public GameObject weaponDefault;

	private Character m_Character;

	public Character character {
		get { return m_Character; }
		set { 
			if (character != null) 
			{
				if (value != null)
					Debug.Log("trying to set character, but there's already a character!");

				Game.Camera_.Unbind(GetHashCode());
				character.GetComponent<Destroyable>().postDestroy -= ListenDestroy;
			    Remove(character.id);
			}

			m_Character = value;

			if (m_Character != null) 
			{
				m_Character.ownerPlayer = Network.player.guid;
				m_Character.renderer_.SetColor(characterColor);

				Game.Camera_.Bind(GetHashCode(), m_Character.transform);

				if (weaponDefault != null)
				{
					var _weapon = (GameObject) Instantiate(weaponDefault);
					m_Character.weapon = _weapon.GetComponent<Weapon>();
				}

				m_Character.postDead += ListenDead;
                m_Character.postObtainCrate += ListenObtainCrate;
                m_Character.GetComponent<Destroyable>().postDestroy += ListenDestroy;
			    Add(m_Character);
			}

			if (postCharacterChanged != null) 
				postCharacterChanged(m_Character);
		}
	}

	public List<Color> characterColors = new List<Color>();
	[HideInInspector]
	public Color characterColor = Color.white;

    public Action<Character> postCharacterChanged;
    public Action<Character> postCharacterDead;

    private readonly Dictionary<int, Character> m_Characters = new Dictionary<int, Character>();
 
    public void Add(Character _character)
    {
        if (! m_Characters.ContainsKey(_character.id))
        {
            m_Characters.Add(_character.id, _character);
            _character.GetComponent<Destroyable>().postDestroy += Remove;
        }
        else
        {
            Debug.LogWarning("Character already exist! Ignore.");
        }
    }

    void Remove(Destroyable _destroyable)
    {
        Remove(_destroyable.GetComponent<Character>().id);
    }

    public void Remove(int _characterID)
    {
        if (m_Characters.ContainsKey(_characterID))
        {
            m_Characters[_characterID].GetComponent<Destroyable>().postDestroy -= Remove;
            m_Characters.Remove(_characterID);
        }
        else 
            Debug.LogWarning("Character does not exist! Ignore.");
    }

	public void Start()
	{
		if (characterColors.Count == 0) 
			characterColors.Add(Color.white);

		characterColor = GenericHelper.SelectRandom (characterColors);
	}

	public void Purge()
	{
		character = null;
	}

	void Update () 
	{
		if (character == null) return;

		var _vertical = Input.GetAxis("Vertical");
		if (_vertical < -0.05f)
			character.rigidbody2D.AddForce(_vertical * downForce * Vector2.up);

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
				character.Stand();
			else 
				character.Crouch();
		}

		if (Input.GetButtonDown("Fire1")) 
		{
			if (character.shootable) 
				character.Shoot();
		}
		else if (Input.GetButtonUp("Fire1")) 
		{
			if (character.weapon != null)
				character.weapon.Rest();
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

	void FixedUpdate() 
	{
		if (character == null) return;

		if (character.movable) 
		{
			var _horizontal = Input.GetAxis("Horizontal");
			if (! _horizontal.Equals(0)) 
				character.Move(_horizontal);
			
			if (Input.GetButton("Jump")) 
			{
				if (character.floating && m_UpForceLeft > 0) 
				{
					m_UpForceLeft -= upForce * Time.fixedDeltaTime;
					m_Character.rigidbody2D.AddForce(new Vector2(0, upForce));
				}
			}
		}
		
	}

	public void ListenDestroy(Destroyable _destroyable) 
	{
		character = null;
	}

	public void ListenDead(Character _character)
	{
		if (_character != character) 
        {
			Debug.LogWarning("Dead character is not mine! Ignore.");
			return;
		}

	    if (Network.peerType == NetworkPeerType.Disconnected)
	    {
            GameCharacter_OnDead(
                Network.player.guid,
                _character.id,
                _character.lastAttackData.Serialize());
	    }
	    else
	    {
            networkView.RPC("GameCharacter_OnDead", RPCMode.All,
                Network.player.guid,
                _character.id,
                _character.lastAttackData.Serialize());
	    }
	}

    [RPC]
    public void GameCharacter_OnDead(string _player, int _characerID, string _attackDataSerial)
    {
        var _attackData = AttackData.Deserialize(_attackDataSerial);

        if (! String.IsNullOrEmpty(_attackData.ownerPlayer))
        {
            var _statistic = Game.Statistic[_attackData.ownerPlayer];
            _statistic.kill.Add(_characerID);
        }

        Game.Statistic[_player].death.Add(_characerID);

        if (postCharacterDead != null)
        {
            Character _character;
            if (m_Characters.TryGetValue(_characerID, out _character))
            {
                _character.lastAttackData = _attackData;
                postCharacterDead(_character);
            }
            else
            {
                Debug.LogWarning("Dead character " + _characerID + " does not exist! Cannot post.");
            }
        }
    }
}
