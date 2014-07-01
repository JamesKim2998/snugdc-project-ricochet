using UnityEngine;
using System.Collections;

public static class NetworkManager {

	private static string m_GameType = "SuperCrateBox_clone";
	public static int connection = 4;
	public static int port = 23489;

	public static string editorMasterServerIP;
	public static string masterServerIP {
		get { return MasterServer.ipAddress; }
		set { 
			if (Network.peerType != NetworkPeerType.Disconnected) {
				Debug.Log("trying to change master server address while server running. ignore.");
				return;
			}
			MasterServer.ipAddress = value;
		}
	}

	public static int editorMasterServerPort = -1;
	public static int masterServerPort {
		get { return MasterServer.port; }
		set { 
			if (Network.peerType != NetworkPeerType.Disconnected) {
				Debug.Log("trying to change master server port while server running. ignore.");
				return;
			}
			MasterServer.port = value;
		}
	}

	public static string natFacilitatorIP {
		get { return Network.natFacilitatorIP; }
		set {
			if (Network.peerType != NetworkPeerType.Disconnected) {
				Debug.Log("trying to change facilitator IP while server running. ignore.");
				return;
			}
			Network.natFacilitatorIP = value;
		}
	}
	
	public static int natFacilitatorPort {
		get { return Network.natFacilitatorPort; }
		set {
			if (Network.peerType != NetworkPeerType.Disconnected) {
				Debug.Log("trying to change facilitator port while server running. ignore.");
				return;
			}
			Network.natFacilitatorPort = value;
		}
	}

	public delegate void PostBeforeDisconnected();
	public static event PostBeforeDisconnected postBeforeDisconnected;

	static NetworkManager() 
	{
		if (editorMasterServerIP != null) 
			MasterServer.ipAddress = masterServerIP;

		if (editorMasterServerPort > 0)
			MasterServer.port = masterServerPort;
	}

	public static void StartServer(string _room) {
		Network.InitializeServer(connection, port, false);//! Network.HavePublicAddress());
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
