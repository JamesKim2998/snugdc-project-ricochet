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

	public int weaponPickUp { set { if (weaponPickUpLabel != null) weaponPickUpLabel.text = value.ToString(); } }
	public int kill { set { if (killLabel != null) killLabel.text = value.ToString(); } }
	public int death { set { if (deathLabel != null) deathLabel.text = value.ToString(); } }
	public int score { set { if (scoreLabel != null) scoreLabel.text = value.ToString(); } }

	public UI2DSprite iconSprite;
	public UILabel nameLabel;
	public UILabel weaponPickUpLabel;
	public UILabel killLabel;
	public UILabel deathLabel;
	public UILabel scoreLabel;

	void Start ()
	{

	}

	void Update ()
	{

	}
}

