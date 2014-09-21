using UnityEngine;
using System.Collections;

public class HUDPlayerName : MonoBehaviour
{
    public UILabel nameLabel;
    public Vector2 offset = new Vector2(0, -80);

    private Character m_Character;
    public Character character
    {
        get { return m_Character;  }
        set
        {
            if (character == value) 
                return;

            if (m_Character)
                m_Character.GetComponent<Destroyable>().postDestroy -= ListenCharacterDestroy;

            m_Character = value;

            if (m_Character)
            {
                m_Character.GetComponent<Destroyable>().postDestroy += ListenCharacterDestroy;
                enabled = true;

                var _playerInfo = Global.Player()[value.ownerPlayer];
                if (_playerInfo == null) return;
                nameLabel.text = _playerInfo.name;
            }
            else
            {
                nameLabel.text = "";
                enabled = false;
            }
        }
    }

    void Start()
    {
        enabled = false;
        if (!nameLabel) nameLabel = GetComponent<UILabel>();
        nameLabel.text = "";
        Game.Character.postCharacterChanged += ListenCharacterChanged;
    }

    void OnDestroy()
    {
        character = null;
    }

	void Update ()
	{
	    if (!character) return;
        var _characterPosition = Game.Camera_.camera.WorldToScreenPoint(m_Character.transform.position);
        nameLabel.transform.localPosition = new Vector3(_characterPosition.x + offset.x, _characterPosition.y + offset.y);
	}

    void ListenCharacterChanged(Character _character)
    {
        character = _character;
    }

    void ListenCharacterDestroy(Destroyable _destroyable)
    {
        character = null;
    }
}
