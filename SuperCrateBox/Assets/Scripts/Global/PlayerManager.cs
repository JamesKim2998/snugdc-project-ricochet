using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
	public string guid = "";

	private string m_Name = "undefined";
	public string name { 
		get { return m_Name; } 
		set { 
			if (m_Name == value) return; 
			m_Name = value; 
			if (postChanged != null) postChanged(this); 
		}
	}

	[System.NonSerialized]
	public Action<PlayerInfo> postChanged;
}

public class PlayerManager : MonoBehaviour
{
	private Dictionary<string, PlayerInfo> m_Players;
	public Dictionary<string, PlayerInfo> players { get { return m_Players; }}

	public Action<bool, string> postConnected;
	public Action<PlayerInfo> postSetuped;
	public Action<PlayerInfo> postInfoChanged;

	private PlayerInfo m_Mine;
	[HideInInspector]
	public PlayerInfo mine { get { return m_Mine;}}

	private PlayerInfo m_Server;
	[HideInInspector]
	public PlayerInfo server { 
		get { 
			if (m_Server == null)
				m_Server = Get(Global.Server().server);
			return m_Server; 
		}
	}

	public bool IsMine(PlayerInfo _player) { return _player.guid == m_Mine.guid; }
	public bool IsServer(PlayerInfo _player ) { if (server != null) return _player.guid == server.guid; return false;}
	public bool IsClient(PlayerInfo _player ) { if (server != null) return _player.guid != server.guid; return false;}

	void Awake() 
	{
		m_Mine = new PlayerInfo();
		m_Mine.guid = Network.player.guid;
		m_Mine.name = Network.player.guid;

		m_Players = new Dictionary<string, PlayerInfo>();
		m_Players.Add(Network.player.guid, m_Mine);	
	}

	void Start()
	{
		Global.Server().postServerInitialized += ListenServerInitialized;
		Global.Server().postConnectionSetuped += ListenConnectionSetuped;
		Global.Server().postDisconnected += ListenDisconnectedFromServer;
		
	}

	void OnDestroy()
	{
		Global.Server().postServerInitialized -= ListenServerInitialized;
		Global.Server().postConnectionSetuped -= ListenConnectionSetuped;
		Global.Server().postDisconnected -= ListenDisconnectedFromServer;
	}

	public PlayerInfo Get(string _player)
	{
		if (! players.ContainsKey(_player))
			return null;
		return players[_player];
	}

	void Add(string _player)
	{
		if (! m_Players.ContainsKey(_player))
		{
			PlayerInfo _playerInfo;
			
			if (_player == Network.player.guid) 
			{
				_playerInfo = mine;
			}
			else 
			{
				_playerInfo = new PlayerInfo();
			}
			
			_playerInfo.guid = _player;
			_playerInfo.name = "player-" + _player;
			
			m_Players[_player] = _playerInfo;
			if (postConnected != null) postConnected(true, _playerInfo.guid);
		}
	}

	void Remove(string _player)
	{
		PlayerInfo _playerInfo = m_Players[_player];
		
		if (_playerInfo == null) 
		{
			Debug.LogError("Disconnected player does not exist!");
			return;
		}
		
		if (postConnected != null) postConnected(false, _playerInfo.guid);

		m_Players.Remove(_player);
	}

	void ListenConnectionSetuped() 
	{
		if (! m_Players.ContainsKey(Network.player.guid)) 
			m_Players.Add(Network.player.guid, m_Mine);

		if (postConnected != null) postConnected(true, m_Mine.guid);
		if (postSetuped != null) postSetuped(m_Mine);

		UpdateInfo();
	}

	void ListenDisconnectedFromServer()
	{
		m_Server = null;

		List<string> _players = new List<string>(m_Players.Keys);

		foreach (var _player in _players)
		{
			if (_player == Network.player.guid) continue;
			Remove(_player);
		}
	}

	void ListenServerInitialized()
	{
		networkView.RPC("PlayerManager_OnPlayerConnected", RPCMode.All, Network.player.guid);
	}

	void OnPlayerConnected(NetworkPlayer _player) 
	{
		if (Network.isServer)
			networkView.RPC("PlayerManager_OnPlayerConnected", RPCMode.All, _player.guid);
	}

	[RPC]
	void PlayerManager_OnPlayerConnected(string _player)
	{
		Add(_player);

		if (Network.player.guid == _player)
			RefreshPlayerInfo();
	}

	void OnPlayerDisconnected(NetworkPlayer _player)
	{
		networkView.RPC("PlayerManager_OnPlayerDisconnected", RPCMode.All, _player.guid);
	}

	[RPC]
	void PlayerManager_OnPlayerDisconnected(string _player)
	{
		Remove(_player);
	}
	
	void RefreshPlayerInfo()
	{
		if (Network.isServer) return;
		networkView.RPC("PlayerManager_RequestRefreshPlayerInfo", RPCMode.Server, Network.player);
	}
	
	[RPC]
	void PlayerManager_RequestRefreshPlayerInfo(NetworkPlayer _player)
	{
		var _playerInfosStr = NetworkSerializer.Serialize(players);
		networkView.RPC("PlayerManager_ResponseRefreshPlayerInfo", _player, _playerInfosStr);
	}

	[RPC]
	void PlayerManager_ResponseRefreshPlayerInfo(string _playerInfosStr)
	{
		var _newPlayerInfos = new Dictionary<string, PlayerInfo>();
		NetworkSerializer.Deserialize(_playerInfosStr, out _newPlayerInfos);

		foreach (var _playerKV in players )
		{
			if (Network.player.guid == _playerKV.Key) continue;

			if (! _newPlayerInfos.ContainsKey(_playerKV.Key))
		    {
				if (postConnected != null) postConnected(false, _playerKV.Value.guid);
				players.Remove(_playerKV.Key);
			}
		}

		foreach (var _newPlayerKV in _newPlayerInfos)
		{
			if (Network.player.guid == _newPlayerKV.Value.guid) continue;

			var _player = players.ContainsKey(_newPlayerKV.Key) 
				? players[_newPlayerKV.Key]
				: null;

			if (_player == null) 
			{
				_player = _newPlayerKV.Value;
				players.Add(_player.guid, _player);
				if (postConnected != null) postConnected(true, _player.guid);
			}
			else
			{
				_player.name = _newPlayerKV.Value.name;
			}

			if (postSetuped != null) postSetuped(_player);
		}
	}

	void UpdateInfo()
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			UpdateInfoLocal(m_Mine);
		}
		else
		{
			var _playerInfoSerial = NetworkSerializer.Serialize(m_Mine);
			networkView.RPC("PlayerManager_UpdateInfo", RPCMode.All, _playerInfoSerial);
		}
	}

	void UpdateInfoLocal(PlayerInfo _player)
	{
		var _playerLocal = Get(_player.guid);

		if (_playerLocal == _player)
			return;

		if (_playerLocal == null)
		{
			Debug.LogWarning("Trying to update not existing player.");
			return;
		}

		_playerLocal.name = _player.name;

		if (postInfoChanged != null) postInfoChanged(_player);
	}

	[RPC]
	void PlayerManager_UpdateInfo(string _playerInfoSerial)
	{
		PlayerInfo _playerInfo;
		NetworkSerializer.Deserialize(_playerInfoSerial, out _playerInfo);
		UpdateInfoLocal (_playerInfo);
	}
}
