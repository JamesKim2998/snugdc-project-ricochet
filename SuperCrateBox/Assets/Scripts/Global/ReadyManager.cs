using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class ReadyManager : MonoBehaviour 
{
	public bool IsReady() { return m_ReadyInfo.Contains(Network.player.guid); }
	public bool IsReady(string _player) { return m_ReadyInfo.Contains(_player); }
	public bool IsReadyAll() 
	{ 
		foreach (var _playerKV in Global.Player().players)
			if (! IsReady(_playerKV.Key)) return false;
		return true;
	}

	private HashSet<string> m_ReadyInfo;
	public HashSet<string> readyInfo { get { return m_ReadyInfo; } }
	public Action<string, bool> postReady;
	
	private bool m_IsPolling = false;
	public bool isPolling { get { return m_IsPolling; }}
	public Action postPoll;

	public ReadyManager()
	{
		m_ReadyInfo = new HashSet<string>();
		Global.Context ().postChanged += ListenContectChanged;
	}

	~ReadyManager()
	{
		Global.Context ().postChanged -= ListenContectChanged;
	}

	public void PollReadyInfo()
	{
		if (m_IsPolling) return;

		m_IsPolling = true;

		if (Network.isServer) 
		{
			Debug.LogWarning("Please use readyInfo instead if your instance is server.");
		}
		else 
		{
			networkView.RPC( "ReadyManager_PollReadyInfoRequest", RPCMode.Server,
			                Network.player);
		}

		Invoke("AbortPoll", 2f);
	}
	
	void AbortPoll()
	{
		m_IsPolling = false;
		Debug.Log("Abort poll.");
	}

	[RPC]
	private void ReadyManager_PollReadyInfoRequest(NetworkPlayer _player) 
	{
		var _readyInfo = new List<string>(m_ReadyInfo);
		networkView.RPC ("ReadyManager_PollReadyInfoResponse", _player, 
		                 NetworkSerializer.Serialize(_readyInfo));
	}

	[RPC]
	private void ReadyManager_PollReadyInfoResponse(string _readyInfoStr)
	{
		CancelInvoke("AbortPoll");

		m_IsPolling = false;

		var _oldReadyInfo = m_ReadyInfo;
		var _newReadyInfo = new List<string>();
		NetworkSerializer.Deserialize(_readyInfoStr, out _newReadyInfo);
		m_ReadyInfo = new HashSet<string>(_newReadyInfo);

		if (postPoll != null) postPoll();
		
		if (postReady != null)
		{
			var _oldExclusiveReadyInfo = new HashSet<string>(_oldReadyInfo);
			_oldExclusiveReadyInfo.ExceptWith(m_ReadyInfo);

			var _newExclusiveReadyInfo = new HashSet<string>(m_ReadyInfo);
			_newExclusiveReadyInfo.ExceptWith(_oldReadyInfo);

			foreach (var _readyInfo in _oldExclusiveReadyInfo)
				postReady (_readyInfo, false);

			foreach (var _readyInfo in _newExclusiveReadyInfo)
				postReady(_readyInfo, true);
		}
			
	}
	
	void ReadyLocal(string _player, bool _ready)
	{
		var _oldReady = m_ReadyInfo.Contains(_player);

		if (_ready == _oldReady)
		{
			Debug.Log("Trying to (un)ready again!");
			return;
		}

		if (_ready)
		{
			m_ReadyInfo.Add(_player);
		}
		else
		{
			m_ReadyInfo.Remove(_player);
		}
		
		if (postReady != null)
			postReady (_player, _ready);
	}

	public void RequestReady(bool _ready) 
	{
		if (Network.isServer)
		{
			Debug.LogError("Server should be always ready.");
			return;
		}

		networkView.RPC ("ReadyManager_ReadyResponse", RPCMode.All, 
		                 Network.player.guid, _ready);
	}

	[RPC]
	private void ReadyManager_ReadyResponse(string _player, bool _ready) 
	{
		ReadyLocal(_player, _ready);
	}

	void OnConnectedToServer() 
	{
		PollReadyInfo();
	}

	void OnDisconnectedFromServer()
	{
		readyInfo.Clear();
		ReadyLocal(Network.player.guid, false);
	}

	void OnServerInitialized()
	{
		ReadyLocal(Network.player.guid, true);
	}

	public void ListenContectChanged(ContextType _context, ContextType _old)
	{
		// note: game에 진입시 ready 정보를 제거합니다.
		if (_context == ContextType.GAME)
		{
			m_ReadyInfo.Clear();
		}
	}
}
