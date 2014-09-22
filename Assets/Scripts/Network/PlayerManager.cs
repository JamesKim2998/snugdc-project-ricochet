using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class PlayerManager : MonoBehaviour, IEnumerable<KeyValuePair<string, PlayerInfo>>
{
    #region player infos
    public Dictionary<string, PlayerInfo> players { get; private set; }

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
    #endregion

    #region predefined players
    public PlayerInfo mine { get; private set; }

    private PlayerInfo m_Server;
	public PlayerInfo server {
        get { return m_Server ?? (m_Server = Get(Global.Server().server)); }
	}

	public bool IsMine(PlayerInfo _player) { return _player.guid == mine.guid; }
	public bool IsServer(PlayerInfo _player ) { if (server != null) return _player.guid == server.guid; return false;}
	public bool IsClient(PlayerInfo _player ) { if (server != null) return _player.guid != server.guid; return false;}

    #endregion

    #region events
    public Action<bool, string> postConnected;
    public Action<PlayerInfo> postSetuped;

    public Action<PlayerInfo> postInfoChanged;
    public Action<string, bool> postReady;
    #endregion

    void Awake()
    {
        mine = new PlayerInfo(Network.player.guid);
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

	#region add, remove
	PlayerInfo Add(string _player)
	{
	    PlayerInfo _playerInfo;
	    if (players.TryGetValue(_player, out _playerInfo))
	    {
            return _playerInfo;
	    }
	    else
	    {
	        _playerInfo = new PlayerInfo(Network.player.guid);
	        Add(_playerInfo);
            return _playerInfo;
        }
	}

    void Add(PlayerInfo _playerInfo)
    {
        PlayerInfo _playerInfoOld = null;

        if (players.TryGetValue(_playerInfo.guid, out _playerInfo))
        {
            Debug.LogWarning("Player already exists! Sync.");
            Copy(_playerInfoOld, _playerInfo);
        }
        else
        {
            players.Add(_playerInfo.guid, _playerInfo);
            m_ListenReadyChanged[_playerInfo.guid] = _value => postReady(_playerInfo.guid, _value.val);
            _playerInfo.isReady.postChanged += m_ListenReadyChanged[_playerInfo.guid];
        }
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
        _playerInfo.isReady.postChanged -= m_ListenReadyChanged[_playerInfo.guid];
	    m_ListenReadyChanged.Remove(_playerInfo.guid);
	}

    private readonly Dictionary<string, Action<ObservableValue<bool>>> m_ListenReadyChanged
        = new Dictionary<string, Action<ObservableValue<bool>>>();
	#endregion

    #region connect, disconnect
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
    #endregion

    static void Copy(PlayerInfo _dst, PlayerInfo _org)
    {
        if (_dst == _org)
        {
            Debug.LogWarning("Trying to copy into same value. Ignore.");
            return;
        }

        _dst.name = _org.name;
        _dst.isReady.val = _org.isReady.val;
        _dst.characterSelected.val = _org.characterSelected.val;
    }


	#region handshake
	void ListenConnectionSetuped() 
	{
	    if (! players.ContainsKey(mine.guid))
            Add(mine);

	    Connect(mine);

        networkView.RPC("PlayerManager_RequestPlayerConnected", RPCMode.Others, Network.player);
	}

	void ListenDisconnectedFromServer()
	{
		m_Server = null;
        
		foreach (var _playerKV in players)
            Disconnect(_playerKV.Value);
	}

	void ListenServerInitialized()
	{
	    ListenConnectionSetuped();
	}

	[RPC]
	void PlayerManager_RequestPlayerConnected(NetworkPlayer _player)
	{
        var _playerInfo = Get(_player.guid) ?? Add(_player.guid);
	    Connect(_playerInfo);
	    if (server == _player.guid)
            networkView.RPC("PlayerMangaer_ResponseOnPlayerConnected", _player);
	}

    [RPC]
    void PlayerMangaer_ResponseOnPlayerConnected()
    {
        if (postSetuped != null) postSetuped(mine);
        UpdateInfo();
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
    #endregion

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

		foreach (var _newPlayerKV in _newPlayerInfos
            .Where(_newPlayerKV => Network.player.guid != _newPlayerKV.Value.guid))
		{
		    PlayerInfo _player;
		    if (!players.TryGetValue(_newPlayerKV.Key, out _player)) 
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

