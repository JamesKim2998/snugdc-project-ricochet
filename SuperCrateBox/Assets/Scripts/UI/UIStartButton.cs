using UnityEngine;
using System.Collections;

public class UIStartButton : MonoBehaviour 
{
	public UIButton button;
	public UILabel label;
	public string map;
	
	private bool m_IsStartable = false;
	public bool isStartable { 
		get { return m_IsStartable; }
		private set { 
			if (m_IsStartable == value) return;
			m_IsStartable = value; 
			button.isEnabled = m_IsStartable;
			label.text = m_IsStartable ? "start" : "wait";
		}
	}

	void Start () 
	{
		gameObject.SetActive(Network.isServer);
		m_IsStartable = Global.Ready().IsReadyAll();

		Global.Server().postServerInitialized += ListenServerInitialized;
		Global.Server().postDisconnected += ListenDisconnectedFromServer;
		Global.Player().postConnected += ListenPlayerConnected;
		Global.Ready().postReady += ListenReady;
	}

	void OnDestroy()
	{
		Global.Server().postServerInitialized -= ListenServerInitialized;
		Global.Server().postDisconnected -= ListenDisconnectedFromServer;
		Global.Player().postConnected -= ListenPlayerConnected;
		Global.Ready().postReady -= ListenReady;
	}

	void StartGame()
	{
		var _transition = new GameTransition().Map(map);
		Global.Transition().RequestStartGame(_transition);
	}

	void Refresh()
	{
		if (Global.Ready().IsReadyAll())
			isStartable = true;
	}
	
	public void OnSubmit()
	{
		StartGame();
//		button.isEnabled = false;	
	}

	void ListenServerInitialized()
	{
		gameObject.SetActive(true);
		Refresh();
	}

	void ListenDisconnectedFromServer()
	{
		gameObject.SetActive(false);
	}

	void ListenPlayerConnected(PlayerInfo _player, bool _connected)
	{
		Refresh();
	}

	void ListenReady(string _player, bool _ready)
	{
		if (_ready) 
		{
			Refresh();
		}
		else
		{
			isStartable = false;
		}
	}
}
