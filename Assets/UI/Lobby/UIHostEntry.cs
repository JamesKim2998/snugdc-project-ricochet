using UnityEngine;
using System.Collections;

public class UIHostEntry : MonoBehaviour {

	public UILabel ipLabel;
	public UILabel portLabel;
	public UILabel nameLabel;
	public UICharacterSelector characterSelector;
	
	public string ip { set { if (ipLabel != null ) ipLabel.text = value; } }
	public int port { set { if (portLabel != null ) portLabel.text = value == 0 ? "" : value.ToString(); } }
	public string name_ { set { if (nameLabel != null ) nameLabel.text = value; } }

	void Start () 
	{
		if (Network.peerType != NetworkPeerType.Disconnected)
			Refresh();

		Global.Player().postSetuped += ListenPlayerSetuped;
	}

	void OnDestroy()
	{
		Global.Player().postSetuped -= ListenPlayerSetuped;
	}

	void Refresh()
	{
		if (Network.peerType != NetworkPeerType.Disconnected)
		{
			ip = Global.Server().ip;
			port = Global.Server().port;
			
			if (Global.Player().server != null)
			{
				name_ = Global.Player().server.name;
			}
		}
	}

	void Clear()
	{
		ip = "";
		port = 0;
		name_ = "";
	}

	void OnServerInitialized()
	{
		characterSelector.player = Network.player.guid;
		Refresh();
	}

	void ListenPlayerSetuped(PlayerInfo _player)
	{
		characterSelector.player = Global.Server().server;
		Refresh();
	}

	void OnDisconnectedFromServer()
	{
		Clear();
	}
}
