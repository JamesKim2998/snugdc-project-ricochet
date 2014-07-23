using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class RoomRequests : MonoBehaviour 
{
	private HashSet<NetworkPlayer> m_ReadyInfo;
	public HashSet<NetworkPlayer> readyInfo { get { return m_ReadyInfo; } }

	public Action<NetworkPlayer, bool> postReady;
	public Action<HashSet<NetworkPlayer>> postPollReadyInfo;

	public RoomRequests()
	{
		m_ReadyInfo = new HashSet<NetworkPlayer> ();
	}

	public void PollReadyInfo()
	{
		if (Network.isServer) 
		{
			Debug.LogWarning("Please use readyInfo instead if your instance is server.");
		}
		else 
		{
			networkView.RPC( "RoomRequests_PollReadyInfo", RPCMode.Server,
			                Network.player);
		}
	}
	
	[RPC]
	private void RoomRequests_PollReadyInfoRequest(NetworkPlayer _player) 
	{
		// string _readyInfo;
		networkView.RPC ("RoomRequests_PollReadyInfoResponse", _player, 
		                 NetworkSerializer.Serialize(m_ReadyInfo));
	}

	[RPC]
	private void RoomRequests_PollReadyInfoResponse(string _readyInfo)
	{

	}

	public void RequestReady(bool _ready) 
	{
		networkView.RPC ("RoomRequests_ReadyRequest", RPCMode.Server, 
		                Network.player, _ready);
	}

	[RPC]
	private void RoomRequests_ReadyRequest(NetworkPlayer _player, bool _ready) 
	{
		networkView.RPC ("RoomRequests_ReadyResponse", RPCMode.All, 
		                 Network.player, _ready);
	}

	[RPC]
	private void RoomRequests_ReadyResponse(NetworkPlayer _player, bool _ready) 
	{
		if (postReady != null)
			postReady (_player, _ready);
	}

}
