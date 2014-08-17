using UnityEngine;

[RequireComponent(typeof(UIInput))]
public class UIChatInput : MonoBehaviour
{
	public UITextList textList;
	public bool fillWithDummyData = false;
	
	public bool isNetworkEnabled { get { return networkView != null && Network.peerType != NetworkPeerType.Disconnected; }}

	UIInput m_Input;
	
	void Start ()
	{
		m_Input = GetComponent<UIInput>();
		m_Input.label.maxLineCount = 1;
	}

	public void OnSubmit ()
	{
		if (textList != null)
		{
			string _text = NGUIText.StripSymbols(m_Input.value);
			
			if (! string.IsNullOrEmpty(_text))
			{
				UIChatInput_OnSubmit(Network.player.guid, _text);

				m_Input.value = "";
				m_Input.isSelected = false;

				if (isNetworkEnabled)
					networkView.RPC("UIChatInput_OnSubmit", RPCMode.Others, Network.player.guid, _text);
			}
		}
	}

	[RPC]
	void UIChatInput_OnSubmit(string _player, string _msg) {
		if (textList == null)
			return;

		var _playerInfo = Global.Player().Get(_player);

		if (_playerInfo != null)
		{
			_msg = "[" + _playerInfo.name + "]: " + _msg;
		}
		else
		{
			Debug.LogWarning("Player is not found.");
		}

		textList.Add(_msg);
	}
}
