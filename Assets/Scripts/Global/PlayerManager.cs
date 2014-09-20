using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using UnityEngine;

[System.Serializable]
public class PlayerInfo
{
	public string guid = "";

    public bool connected = false;

    private string m_Name = "undefined";
	public string name { 
		get { return m_Name; } 
		set { 
			if (m_Name == value) return; 
			m_Name = value; 
			if (postChanged != null) postChanged(this); 
		}
	}

	private CharacterType m_CharacterSelected = CharacterType.BLUE;
	public CharacterType characterSelected { 
		get { return m_CharacterSelected; } 
		set { 
			if (m_CharacterSelected == value) return; 
			m_CharacterSelected = value; 
			if (postChanged != null) postChanged(this); 
		}
	}

	[System.NonSerialized]
	public Action<PlayerInfo> postChanged;
}

public class PlayerManager : MonoBehaviour
{
    public Dictionary<string, PlayerInfo> players { get; private set; }

    public Action<bool, string> postConnected;
	public Action<PlayerInfo> postSetuped;
	public Action<PlayerInfo> postInfoChanged;

    public PlayerInfo mine { get; private set; }

    private PlayerInfo m_Server;
	public PlayerInfo server { 
		get { return m_Server ?? (m_Server = Get(Global.Server().server)); }
	}

	public bool IsMine(PlayerInfo _player) { return _player.guid == mine.guid; }
	public bool IsServer(PlayerInfo _player ) { if (server != null) return _player.guid == server.guid; return false;}
	public bool IsClient(PlayerInfo _player ) { if (server != null) return _player.guid != server.guid; return false;}

    public PlayerInfo this[string _player]
    {
        get
        {
            PlayerInfo _playerInfo;
            return players.TryGetValue(_player, out _playerInfo)
                ? _playerInfo
                : null;
        }
    }

    void Awake() 
	{
		mine = new PlayerInfo {guid = Network.player.guid, name = Network.player.guid};
	    players = new Dictionary<string, PlayerInfo> { {Network.player.guid, mine} };
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
	    PlayerInfo _playerInfo;
	    return players.TryGetValue(_player, out _playerInfo)
	        ? _playerInfo
	        : null;
	}

	void Add(string _player)
	{
	    var _playerInfo = _player == Network.player.guid ? mine : new PlayerInfo();
	    _playerInfo.guid = _player;
	    _playerInfo.name = "player-" + _player;
        Add(_playerInfo);
	}

    void Add(PlayerInfo _playerInfo)
    {
        if (players.ContainsKey(_playerInfo.guid))
        {
            Debug.LogWarning("Player already exists! Ignore.");
            return;
        }

        players[_playerInfo.guid] = _playerInfo;
    }

	void Remove(string _player)
	{
		var _playerInfo = players[_player];
		
		if (_playerInfo == null) 
		{
			Debug.LogError("Removing player does not exist!");
			return;
		}

	    if (_playerInfo.connected)
	        Debug.LogWarning("Trying to remove connected player. Sure?");

		players.Remove(_player);
	}

    void Connect(PlayerInfo _playerInfo)
    {
        if (_playerInfo.connected)
        {
            Debug.LogWarning("Player already connected! Ignore.");
            return;
        }

        _playerInfo.connected = true;
        if (postConnected != null) postConnected(true, _playerInfo.guid);
    }

    void Disconnect(PlayerInfo _playerInfo)
    {
        if (! _playerInfo.connected)
        {
            Debug.LogWarning("Player already disconnected! Ignore.");
            return;
        }

        _playerInfo.connected = false;
        if (postConnected != null) postConnected(false, _playerInfo.guid);   
    }

    static void Copy(PlayerInfo _dst, PlayerInfo _org)
    {
        _dst.name = _org.name;
        _dst.characterSelected = _org.characterSelected;
    }

	void ListenConnectionSetuped() 
	{
	    if (! players.ContainsKey(mine.guid))
            Add(mine);

	    Connect(mine);
	    
		if (postSetuped != null) postSetuped(mine);

		UpdateInfo();
	}

	void ListenDisconnectedFromServer()
	{
		m_Server = null;
        
		foreach (var _playerKV in players)
            Disconnect(_playerKV.Value);
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
        Connect(Get(_player));

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
	    var _playerInfo = Get(_player);

	    if (_playerInfo == null)
	    {
	        Debug.Log("Disconnected player does not exist! Ignore.");
	        return;
	    }

        Disconnect(_playerInfo);
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
		Dictionary<string, PlayerInfo> _newPlayerInfos;
		NetworkSerializer.Deserialize(_playerInfosStr, out _newPlayerInfos);

		foreach (var _playerKV in players
            .Where(_playerKV => Network.player.guid != _playerKV.Key)
            .Where(_playerKV => ! _newPlayerInfos.ContainsKey(_playerKV.Key)))
		{
            Disconnect(_playerKV.Value);
            Remove(_playerKV.Key);
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

			    if (_player.connected)
			    {
			        _player.connected = false;
                    Connect(_player);
                }
			}
			else
			{
                Copy(_player, _newPlayerKV.Value);

			    if (_player.connected != _newPlayerKV.Value.connected)
			    {
			        if (_newPlayerKV.Value.connected)
                        Connect(_player);
			        else
                        Disconnect(_player);
			    }
			}

			if (postSetuped != null) postSetuped(_player);
		}
	}

	public void UpdateInfo()
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
			UpdateInfoLocal(mine);
		}
		else
		{
			var _playerInfoSerial = NetworkSerializer.Serialize(mine);
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
			Debug.LogWarning("Trying to update not existing player. Ignore.");
			return;
		}

        Copy(_playerLocal, _player);

	    if (_playerLocal.connected != _player.connected)
	    {
            if (_player.connected)
                Connect(_playerLocal);
            else
                Disconnect(_playerLocal);
	    }

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

