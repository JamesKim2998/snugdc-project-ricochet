using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
	public string guid = "";

	private string m_Name = "anonymous";
	public string name { get { return m_Name; } set { m_Name = value; if (postChanged != null) postChanged(this); }}

	[System.NonSerialized]
	public Action<PlayerInfo> postChanged;
}

public class PlayerManager : MonoBehaviour
{
	private Dictionary<string, PlayerInfo> m_Players;
	public Dictionary<string, PlayerInfo> players { get { return m_Players; }}

	public Action<PlayerInfo, bool> postConnected;
	public Action<PlayerInfo> postInfoChanged;

	public PlayerInfo m_Mine;

	[HideInInspector]
	public PlayerInfo mine { get { return m_Mine;}}

	void Awake() 
	{
		m_Mine = new PlayerInfo();
		m_Mine.guid = Network.player.guid;
		m_Mine.name = Network.player.guid;
		m_Mine.postChanged += ListenInfoChanged;

		m_Players = new Dictionary<string, PlayerInfo>();
		m_Players.Add(Network.player.guid, m_Mine);	
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
				_playerInfo.postChanged += ListenInfoChanged;
			}
			
			_playerInfo.guid = _player;
			_playerInfo.name = "player-" + _player;
			
			m_Players[_player] = _playerInfo;
			if (postConnected != null) postConnected(_playerInfo, true);
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
		
		if (_playerInfo != mine) 
			_playerInfo.postChanged -= ListenInfoChanged;
		
		if (postConnected != null) postConnected(_playerInfo, false);

		m_Players.Remove(_player);
	}

	void OnConnectedToServer() 
	{
		if (! m_Players.ContainsKey(Network.player.guid)) 
			m_Players.Add(Network.player.guid, m_Mine);
	}

	void OnDisconnectedFromServer()
	{
		List<string> _players = new List<string>(m_Players.Keys);

		foreach (var _player in _players)
		{
			if (_player == Network.player.guid) continue;
			Remove(_player);
		}
	}

	void OnPlayerConnected(NetworkPlayer _player) 
	{
		networkView.RPC("PlayerManager_OnPlayerConnected", RPCMode.All, _player.guid);
	}

	[RPC]
	void PlayerManager_OnPlayerConnected(string _player)
	{
		Add(_player);

		if (Network.player.guid == _player)
			RefreshPlayerInfo();
	}

	[RPC]
	void PlayerManager_ResponseOnPlayerConnected()
	{
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
				if (postConnected != null) postConnected(_playerKV.Value, false);
				_playerKV.Value.postChanged -= ListenInfoChanged;
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
				_player.postChanged = ListenInfoChanged;
				players.Add(_player.guid, _player);
				if (postConnected != null) postConnected(_player, true);
			}
			else
			{
				_player.name = _newPlayerKV.Value.name;
			}
		}
	}

	void ListenInfoChanged(PlayerInfo _player)
	{
		if (postInfoChanged != null) postInfoChanged(_player);
	}

}
