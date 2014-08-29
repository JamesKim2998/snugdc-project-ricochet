using UnityEngine;
using System;
using System.Collections;

// server 연결 초기화는 
// 연결 -> server guid 취득 -> setup post
// 순으로 진행됩니다.
public class ServerManager : MonoBehaviour
{
	public string server = "";
	public string ip;
	public int port;

	public int defaultConnections = 4;

	public Action postServerInitialized;
	public Action postConnected;
	public Action postConnectionSetuped;
	public Action postDisconnected;

	public void Initiate(int _port) 
	{
		Initiate(defaultConnections, _port, false);
	}
	
	public void Initiate(int _connections, int _port, bool _useNat) 
	{
		server = Network.player.guid;
		ip = Network.player.ipAddress;
		port = _port;
		Network.InitializeServer(_connections, _port, _useNat);
	}

	public void Disconnect()
	{
		Network.Disconnect();
	}

	public void Connect(string _ip, int _port)
	{
		ip = _ip;
		port = _port;
		Network.Connect(_ip, _port);
	}

	void OnServerInitialized()
	{
		if (postServerInitialized != null) postServerInitialized();
		// if (postConnectionSetuped != null) postConnectionSetuped();
	}
	
	void OnConnectedToServer()
	{
		if (postConnected != null) postConnected();
		networkView.RPC("ServerManager_RequestServerGUID", RPCMode.Server, Network.player);
	}

	void OnDisconnectedFromServer()
	{
		if (postDisconnected != null) postDisconnected();
		// Network.RemoveRPCs(Network.player);
	}

	[RPC]
	void ServerManager_RequestServerGUID(NetworkPlayer _requestor)
	{
		networkView.RPC("ServerManager_ResponseServerGUID", _requestor, Network.player.guid);
	}

	[RPC]
	void ServerManager_ResponseServerGUID(string _serverGUID)
	{
		server = _serverGUID;
		if (postConnectionSetuped != null)
			postConnectionSetuped();
	}
}

