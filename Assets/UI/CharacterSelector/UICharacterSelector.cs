using UnityEngine;
using System.Collections;

public class UICharacterSelector : MonoBehaviour 
{
	public UI2DSprite icon;
	public GameObject leftButton;
	public GameObject rightButton;

	private CharacterType m_Type = CharacterType.NONE;
	public CharacterType type {
		get { return m_Type; }
		set {
			if (m_Type == value) return;

			m_Type = value;

			if (m_Type == CharacterType.NONE) 
			{
				Debug.LogWarning("Trying to set type NONE. Ignore.");
				return;
			}

			icon.sprite2D = Database.Skin.GetIcon(m_Type);

			if (player == null)
			{
				Debug.LogWarning("Player is not specified. Ignore.");
				return;
			}

			Global.Player()[player].characterSelected.val = m_Type;
			Global.Player().UpdateInfo();
		}
	}
	
	private string m_Player;
	public string player {
		get { return m_Player; }
		set {
			if (m_Player == value) return;

			if (m_Player != null) 
                Global.Player()[m_Player].characterSelected.postChanged -= ListenCharacterSelectedChanged;

			m_Player = value;

			if (m_Player != null) 
			{
				var _canSelect = m_Player == Network.player.guid;
				leftButton.SetActive(_canSelect);
				rightButton.SetActive(_canSelect);

				type = Global.Player()[m_Player].characterSelected;
				Global.Player()[m_Player].characterSelected.postChanged += ListenCharacterSelectedChanged;
			}
		}
	}

	void Awake()
	{
		if (! icon)
		{
			LogCommon.MissingComponent();
			return;
		}
	}

	public void left() {
		if (type == CharacterType.NONE) return;

		if (type - 1 == CharacterType.BEGIN)
		{
			type = CharacterType.END - 1;
		}
		else
		{
			--type;
		}
	}

	public void right() {
		if (type == CharacterType.NONE) return;

		if (type + 1 == CharacterType.END)
		{
			type = CharacterType.BEGIN + 1;
		}
		else
		{
			++type;
		}
	}

	public void ListenCharacterSelectedChanged(ObservableValue<CharacterType> _value) 
	{
        type = _value;
	}
}
