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
    }

    void OnDestroy()
    {
        character = null;
    }

	void Update ()
	{
	    if (!character) return;
        var _viewportPosition = Game.Camera_.camera.WorldToViewportPoint(character.transform.position);
        nameLabel.transform.position = GameHUD.mainLayer.camera_.ViewportToWorldPoint(_viewportPosition);
        nameLabel.transform.localPosition += new Vector3(offset.x , offset.y);
	}

    void ListenCharacterDestroy(Destroyable _destroyable)
    {
        character = null;
        Destroy(gameObject);
    }
}
