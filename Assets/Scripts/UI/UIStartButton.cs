using UnityEngine;
using System.Collections;

public class UIStartButton : MonoBehaviour 
{
	public UIButton button;
	public UILabel label;
	
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
		Global.Player().postConnected += ListenPlayerConnected;
		Global.Ready().postReady += ListenReady;
	}

	void OnDestroy()
	{
		Global.Server().postServerInitialized -= ListenServerInitialized;
		Global.Player().postConnected -= ListenPlayerConnected;
		Global.Ready().postReady -= ListenReady;
	}

	void StartGame()
	{
		if (! Global.GameSetting().valid)
		{
			Debug.LogWarning("GameSetting is not valid.");
			return;
		}

		var _map = Global.GameSetting().map;
		var _mode = Global.GameSetting().modeSelected;
		_mode.overrideMode = true;

		var _transition = new GameTransition()
			.Map(_map)
			.Mode(_mode);

		Global.Transition().RequestStartGame(_transition);
	}

	void Refresh()
	{
		isStartable = Global.Ready().IsReadyAll();
	}
	
	public void OnSubmit()
	{
		// startable 값이 잘못 설정되는 경우가 있어서 refresh 합니다.
		Refresh();

		if (isStartable) 
			StartGame();
	}

	void ListenServerInitialized()
	{
		Refresh();
	}

	void ListenPlayerConnected(bool _connected, string _player)
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
