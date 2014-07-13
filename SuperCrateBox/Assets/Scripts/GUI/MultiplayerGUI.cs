using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MultiplayerGUI : MonoBehaviour {

	public GameObject credit;
	enum Context {
		Main,
		Singleplayer,
		Multiplayer,
		RoomList,
		Credit,
	}

	Context m_Context;
	Stack<Context> m_ContextStack;

	int m_DirectHostPort = 1234;
	string m_DirectJoinIP = "127.0.0.1";
	int m_DirectJoinPort = 1234;

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
			if (m_HostList.Length == 0) 
				NetworkManager.RequestHostList();
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
		case Context.Credit:	   DisplayCredit();		break;
		}
	}

	void DisplayCredit()
	{
		credit.SetActive (true);
		
		if (GUILayout.Button("Back")){
			credit.SetActive (false);
			Back();
		}
	}

	void DisplayBack() 
	{

		if (GUILayout.Button("Back")) {
			Back();
		}
	}

	void DisplayMain() 
	{
		
		GUILayout.BeginVertical();

		if (GUILayout.Button("SinglePlayer"))
			Enter(Context.Singleplayer);

		if (GUILayout.Button("MultiPlayer")) 
			Enter(Context.Multiplayer);
		
		if (GUILayout.Button("Credit")) 
			Enter(Context.Credit);
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

		GUILayout.BeginHorizontal ();
		GUILayout.Label("DirectPort");
		m_DirectHostPort = int.Parse(GUILayout.TextField (m_DirectHostPort.ToString()));
		if (GUILayout.Button("Host"))
			Network.InitializeServer(4, m_DirectHostPort, false);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label ("DirectAddress");
		m_DirectJoinIP = GUILayout.TextField (m_DirectJoinIP);
		m_DirectJoinPort = int.Parse(GUILayout.TextField (m_DirectJoinPort.ToString()));
		if (GUILayout.Button("Join"))
			Network.Connect(m_DirectJoinIP, m_DirectJoinPort);
		GUILayout.EndHorizontal ();

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
			if ( GUILayout.Button(_host.gameName)) 
				Network.Connect(_host);
		}

		GUI.enabled = true;

		DisplayBack();

		GUILayout.EndVertical();
	}

	void OnMasterServerEvent(MasterServerEvent e) 
	{
		if (e == MasterServerEvent.HostListReceived) 
			m_HostList = MasterServer.PollHostList();
	}
}

public class HostButton : MonoBehaviour 
{
	string m_RoomName = "ServerName";

	public void Display() 
	{
		if (Network.isServer) 
		{
			if (GUILayout.Button("Disconnect")) 
				NetworkManager.StopServer();
		} 
		else 
		{
			GUILayout.BeginHorizontal();

			if (Network.isClient)
				GUI.enabled = false;

			if (GUILayout.Button("HostMasterServer")) 
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
				NetworkManager.Disconnect();
		} 
		else 
		{
			if (Network.isServer) 
				GUI.enabled = false;

			if (GUILayout.Button("JoinMasterServer"))
				callback();

			GUI.enabled = true;
		}
	}
}
