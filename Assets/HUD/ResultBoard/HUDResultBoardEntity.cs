using UnityEngine;
using System.Collections;

public class HUDResultBoardEntity : MonoBehaviour
{
	public string player { 
		set {
			var _player = Global.Player().Get(value);
			if (nameLabel != null) nameLabel.text = _player.name;
			// todo: icon 교체할 것.
		} 
	}

    public CharacterType character { set { if (characterIcon != null) characterIcon.sprite2D = Database.Skin.GetIcon(value); } }
	public int weaponPickUp { set { if (weaponPickUpLabel != null) weaponPickUpLabel.text = value.ToString(); } }
	public int kill { set { if (killLabel != null) killLabel.text = value.ToString(); } }
	public int death { set { if (deathLabel != null) deathLabel.text = value.ToString(); } }
	public int score { set { if (scoreLabel != null) scoreLabel.text = value.ToString(); } }

	public UI2DSprite characterIcon;
	public UILabel nameLabel;
	public UILabel weaponPickUpLabel;
	public UILabel killLabel;
	public UILabel deathLabel;
	public UILabel scoreLabel;
}

