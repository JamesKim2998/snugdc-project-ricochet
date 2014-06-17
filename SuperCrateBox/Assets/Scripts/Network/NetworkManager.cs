using UnityEngine;
using System.Collections;

public static class NetworkManager {

	private static string m_GameType = "SuperCrateBox_clone";
	public static int connection = 4;
	public static int port = 23489;

	public delegate void PostBeforeDisconnected();
	public static event PostBeforeDisconnected postBeforeDisconnected;

	public static void StartServer(string _room) {
		Network.InitializeServer(connection, port, ! Network.HavePublicAddress());
		MasterServer.RegisterHost(m_GameType, _room);
	}

	public static void StopServer() {
		MasterServer.UnregisterHost();
		Network.Disconnect();
	}

	public static void RequestHostList() {
		MasterServer.RequestHostList(m_GameType);
	}

	public static void Disconnect() 
	{
		if ( postBeforeDisconnected != null) postBeforeDisconnected();
		Network.Disconnect();
	}
}
