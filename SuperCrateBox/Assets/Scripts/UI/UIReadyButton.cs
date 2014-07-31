using UnityEngine;
using System.Collections;

public class UIReadyButton : MonoBehaviour {
	private bool m_IsReadying = false;
	public UIButton button;
	public UILabel label;

	void Start() 
	{
		if (Network.peerType != NetworkPeerType.Disconnected)
			Global.Ready().PollReadyInfo();

		Global.Server().postDisconnected += ListenDisconnectedFromServer;
		Global.Ready().postPoll += ListenPollReadyInfo;
		Global.Ready().postReady += ListenReady;
	}

	void OnDestroy()
	{
		Global.Server().postDisconnected -= ListenDisconnectedFromServer;
		Global.Ready().postPoll -= ListenPollReadyInfo;
		Global.Ready().postReady -= ListenReady;
	}

	void SetReady(bool _ready) 
	{
		m_IsReadying = false;

		if (button != null)
			button.isEnabled = true;
		
		if (label != null)
			label.text = ! _ready ? "ready" : "unready";
	}

	void ListenPollReadyInfo() 
	{
		SetReady(Global.Ready().IsReady());
	}

	void ListenReady(string _player, bool _ready) 
	{
		if (Network.player.guid != _player)
			return;

		CancelInvoke("AbortReady");
		SetReady(_ready);
	}

	void AbortReady() 
	{
		Debug.Log("Abort ready.");
		SetReady(Global.Ready().IsReady());
	}
	
	void OnServerInitialized()
	{
		gameObject.SetActive(false);
	}

	void OnConnectedToServer() 
	{
		button.isEnabled = true;
	}

	void ListenDisconnectedFromServer()
	{
		button.isEnabled = false;
		gameObject.SetActive(true);
	}

	public void OnSubmit()
	{
		if (Global.Ready().isPolling) 
		{
			Debug.LogWarning("Trying to ready but now in polling! Ignore.");
			return;
		}
		
		if (m_IsReadying ) 
		{
			Debug.LogWarning("Trying to ready but now in progress! Ignore.");
			return;
		}
		
		m_IsReadying = true;
		
		if (button != null)
			button.isEnabled = false;
		
		if (label != null)
			label.text = "patient";
		
		Global.Ready().RequestReady(! Global.Ready().IsReady());
		
		Invoke("AbortReady", 2f);
	}
}
