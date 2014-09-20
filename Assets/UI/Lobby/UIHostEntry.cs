using UnityEngine;
using System.Collections;

public class UIHostEntry : MonoBehaviour
{
    private string m_Player;

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
	    if (Network.peerType == NetworkPeerType.Disconnected)
	        return;

        if (m_Player == null)
            m_Player = Global.Server().server;

        ip = Global.Server().ip;
		port = Global.Server().port;
		name_ = Global.Player().server.name;
        characterSelector.player = m_Player;
    }

	void Clear()
	{
		ip = "";
		port = 0;
		name_ = "";
	}

	void OnServerInitialized()
	{
		m_Player = Network.player.guid;
        Invoke("Refresh", 0.05f);
	}

	void ListenPlayerSetuped(PlayerInfo _player)
	{
        m_Player = null;
        Invoke("Refresh", 0.05f);
	}

	void OnDisconnectedFromServer()
	{
		Clear();
	}
}
