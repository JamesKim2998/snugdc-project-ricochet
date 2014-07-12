using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiplayerGUI : MonoBehaviour {

	enum Context {
		Main,
		Singleplayer,
		Multiplayer,
		RoomList,
	}

	Context m_Context;
	Stack<Context> m_ContextStack;

	HostButton m_HostButton;
	JoinButton m_JoinButton;

	HostData[] m_HostList;

	void Start () {
		m_ContextStack = new Stack<Context>();
		Enter(Context.Main);

		// multiplier
		m_HostButton = gameObject.AddComponent<HostButton>();
		m_JoinButton = gameObject.AddComponent<JoinButton>();
	}
	
	void Update () {
	
	}

	void Enter(Context _context) {
		m_ContextStack.Push(m_Context);
		m_Context = _context;

		switch (m_Context) {
		case Context.RoomList:
			m_HostList = MasterServer.PollHostList();
			if (m_HostList.Length == 0) {
				NetworkManager.RequestHostList();
			}
			break;
		}

	}

	void Back() 
	{
		if (m_ContextStack.Count == 1) 
		{
			Application.Quit();
			return;
		}

		m_Context = m_ContextStack.Pop();
	}

	void OnGUI() 
	{
		switch (m_Context) {
		case Context.Main:         DisplayMain();         break;
		case Context.Singleplayer: DisplaySingleplay();   break;
		case Context.Multiplayer:  DisplayMultiplay();    break;
		case Context.RoomList:     DisplayRoomList();      break;
		}
	}

	void DisplayBack() 
	{
		if (GUILayout.Button("Back"))
			Back();
	}

	void DisplayMain() 
	{
		
		GUILayout.BeginVertical();

		if (GUILayout.Button("SinglePlayer"))
			Enter(Context.Singleplayer);

		if (GUILayout.Button("MultiPlayer")) 
			Enter(Context.Multiplayer);

		if (GUILayout.Button("Exit"))
			Back();

		GUILayout.EndVertical();
	}

	void DisplaySingleplay() 
	{
		GUILayout.BeginVertical();

		if (GUILayout.Button("Play"))
			Application.LoadLevel("level_test");

		DisplayBack();

		GUILayout.EndVertical();
	}
	
	void DisplayMultiplay() 
	{
		GUILayout.BeginVertical();

		m_HostButton.Display();
		m_JoinButton.Display(() => Enter(Context.RoomList));
		DisplayBack();

		GUILayout.EndVertical();
	}

	void DisplayRoomList() 
	{
		GUILayout.BeginVertical();

		if (Network.isClient)
			GUI.enabled = false;

		foreach (var _host in m_HostList) 
		{
			if ( GUILayout.Button(_host.gameName)) {
				Network.Connect(_host);
			}
		}

		GUI.enabled = true;

		DisplayBack();

		GUILayout.EndVertical();
	}

	void OnMasterServerEvent(MasterServerEvent e) 
	{
		if (e == MasterServerEvent.HostListReceived) 
		{
			m_HostList = MasterServer.PollHostList();
		}
	}
}

public class HostButton : MonoBehaviour 
{
	string m_RoomName = "ServerName";

	public void Display() 
	{
		if (Network.isServer) {
			if (GUILayout.Button("Disconnect")) {
				NetworkManager.StopServer();
			}
		} else {
			GUILayout.BeginHorizontal();

			if (Network.isClient)
				GUI.enabled = false;

			if (GUILayout.Button("Host")) 
				NetworkManager.StartServer(m_RoomName);

			m_RoomName = GUILayout.TextField(m_RoomName);

			GUI.enabled = true;

			GUILayout.EndHorizontal();
		}
	}
}

public class JoinButton : MonoBehaviour 
{
	public void Display(System.Action callback)
	{
		if (Network.isClient) 
		{
			if (GUILayout.Button("Disconnect")) 
			{
				NetworkManager.Disconnect();
			}
		} 
		else 
		{
			if (Network.isServer) 
				GUI.enabled = false;

			if (GUILayout.Button("Join"))
				callback();

			GUI.enabled = true;
		}
	}
}
