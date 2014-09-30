using UnityEngine;
using System.Collections;

public static class MasterServerManager {

	private static string m_GameType = "SuperCrateBox_clone";
	public static int connection = 4;
	public static int port = 23489;

	public static string editorMasterServerIP;
	public static string masterServerIP {
		get { return MasterServer.ipAddress; }
		set { 
			if (Network.peerType != NetworkPeerType.Disconnected) {
                Debug.Log("Trying to change master server address while server running. Ignore.");
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
				Debug.Log("Trying to change master server port while server running. Ignore.");
				return;
			}
			MasterServer.port = value;
		}
	}

	public static string natFacilitatorIP {
		get { return Network.natFacilitatorIP; }
		set {
			if (Network.peerType != NetworkPeerType.Disconnected) {
				Debug.Log("Trying to change facilitator IP while server running. Ignore.");
				return;
			}
			Network.natFacilitatorIP = value;
		}
	}
	
	public static int natFacilitatorPort {
		get { return Network.natFacilitatorPort; }
		set {
			if (Network.peerType != NetworkPeerType.Disconnected) {
				Debug.Log("Trying to change facilitator port while server running. Ignore.");
				return;
			}
			Network.natFacilitatorPort = value;
		}
	}

	static MasterServerManager() 
	{
		if (editorMasterServerIP != null) 
			MasterServer.ipAddress = masterServerIP;

		if (editorMasterServerPort > 0)
			MasterServer.port = masterServerPort;
	}

	public static void StartServer(string _room) {
		Global.Server().Initiate(connection, port, false);
		MasterServer.RegisterHost(m_GameType, _room);
	}

	public static void StopServer() {
		MasterServer.UnregisterHost();
		Global.Server().Disconnect();
	}

	public static void RequestHostList() {
		MasterServer.RequestHostList(m_GameType);
	}

}
