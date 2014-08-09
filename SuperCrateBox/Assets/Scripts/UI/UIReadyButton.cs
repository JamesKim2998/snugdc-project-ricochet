using UnityEngine;
using System.Collections;

public class UIReadyButton : MonoBehaviour {
	private bool m_IsReadying = false;

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
	}

	void ListenDisconnectedFromServer()
	{
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
		
		Global.Ready().RequestReady(! Global.Ready().IsReady());
		
		Invoke("AbortReady", 2f);
	}
}
