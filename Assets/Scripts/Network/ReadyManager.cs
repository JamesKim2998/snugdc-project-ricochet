using System.Linq;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class ReadyManager : MonoBehaviour 
{
	public bool IsReady() { return readyInfo.Contains(Network.player.guid); }
	public bool IsReady(string _player) { 
		if (Global.Server ().server == _player)
		{
			if (! readyInfo.Contains(_player))
			{
				Debug.LogWarning("Ready info doesn't contain server. Add.");
				readyInfo.Add(_player);
			}
			return true;
		}
		return readyInfo.Contains(_player); 
	}

	public bool IsReadyAll()
	{
	    return Global.Player().players.All(_playerKV => IsReady(_playerKV.Key));
	}

    public HashSet<string> readyInfo { get; private set; }
    public Action<string, bool> postReady;

    public bool isPolling { get; private set; }
    public Action postPoll;

	public ReadyManager()
	{
	    isPolling = false;
	    readyInfo = new HashSet<string>();
	}

    public void Start()
	{
		Global.Context ().postChanged += ListenContextChanged;
	}

	~ReadyManager()
	{
		Global.Context ().postChanged -= ListenContextChanged;
	}

	public void PollReadyInfo()
	{
		if (isPolling) return;

		isPolling = true;

		if (Network.isServer) 
			Debug.LogWarning("Please use readyInfo instead if your instance is server.");
		else 
			networkView.RPC( "ReadyManager_PollReadyInfoRequest", RPCMode.Server,
			                Network.player);

		Invoke("AbortPoll", 2f);
	}
	
	void AbortPoll()
	{
		isPolling = false;
		Debug.Log("Abort poll.");
	}

	[RPC]
	private void ReadyManager_PollReadyInfoRequest(NetworkPlayer _player) 
	{
		var _readyInfo = new List<string>(readyInfo);
		networkView.RPC ("ReadyManager_PollReadyInfoResponse", _player, 
		                 NetworkSerializer.Serialize(_readyInfo));
	}

	[RPC]
	private void ReadyManager_PollReadyInfoResponse(string _readyInfoStr)
	{
		CancelInvoke("AbortPoll");

		isPolling = false;

		var _oldReadyInfo = readyInfo;
		List<string> _newReadyInfo;
		NetworkSerializer.Deserialize(_readyInfoStr, out _newReadyInfo);
		readyInfo = new HashSet<string>(_newReadyInfo);

		if (postPoll != null) postPoll();
		
		if (postReady != null)
		{
			var _oldExclusiveReadyInfo = new HashSet<string>(_oldReadyInfo);
			_oldExclusiveReadyInfo.ExceptWith(readyInfo);

			var _newExclusiveReadyInfo = new HashSet<string>(readyInfo);
			_newExclusiveReadyInfo.ExceptWith(_oldReadyInfo);

			foreach (var _readyInfo in _oldExclusiveReadyInfo)
				postReady (_readyInfo, false);

			foreach (var _readyInfo in _newExclusiveReadyInfo)
				postReady(_readyInfo, true);
		}
			
	}
	
	void ReadyLocal(string _player, bool _ready)
	{
		var _oldReady = readyInfo.Contains(_player);

		if (_ready == _oldReady)
		{
			Debug.Log("Trying to (un)ready again!");
			return;
		}

		if (_ready)
		{
			readyInfo.Add(_player);
		}
		else
		{
			readyInfo.Remove(_player);
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

	public void ListenContextChanged(ContextType _context, ContextType _old)
	{
		// note: game에 진입시 ready 정보를 제거합니다.
		if (_context == ContextType.GAME)
			readyInfo.Clear();

		readyInfo.Add(Global.Server().server);
	}
}
