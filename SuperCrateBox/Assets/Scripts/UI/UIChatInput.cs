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
			
			if (!string.IsNullOrEmpty(_text))
			{
				textList.Add(_text);
				m_Input.value = "";
				m_Input.isSelected = false;
				
				if (isNetworkEnabled)
					networkView.RPC("UIChatInput_OnSubmit", RPCMode.Others, Network.player, _text);
			}
		}
	}

	[RPC]
	void UIChatInput_OnSubmit(NetworkPlayer _player, string _msg) {
		if (textList != null)
			textList.Add(_msg);
	}
}
