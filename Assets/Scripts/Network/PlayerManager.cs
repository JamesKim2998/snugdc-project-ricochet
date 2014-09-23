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

        if (players.TryGetValue(_playerInfo.guid, out _playerInfoOld))
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

        UpdateInfo(() => RefreshPlayerInfo(() =>
        {
            if (postSetuped != null) postSetuped(mine);
        }));
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

    private bool m_IsRefreshingPlayerInfo = false;
    private Action m_RefreshPlayerInfoCallback;

    void RefreshPlayerInfo(Action _callback)
	{
        if (Network.isServer)
        {
            if (_callback != null) _callback();
            return;
        }

        m_RefreshPlayerInfoCallback += _callback;
        if (m_IsRefreshingPlayerInfo) return;
        m_IsRefreshingPlayerInfo = true;
        networkView.RPC("PlayerManager_RequestRefreshPlayerInfo", RPCMode.Server, Network.player);
	}
	
	[RPC]
	void PlayerManager_RequestRefreshPlayerInfo(NetworkPlayer _player)
	{
	    foreach (var _playerInfo in players)
	    {
	        if (_playerInfo.Key == _player.guid) continue;
            var _playerInfoStr = NetworkSerializer.Serialize(_playerInfo.Value);
            networkView.RPC("PlayerManager_ResponseRefreshPlayerInfo", _player, _playerInfoStr);
	    }
        networkView.RPC("PlayerManager_ResponseRefreshPlayerInfoDone", _player);
    }

	[RPC]
    void PlayerManager_ResponseRefreshPlayerInfo(string _playerInfoStr)
	{
		PlayerInfo _newPlayerInfo;
		NetworkSerializer.Deserialize(_playerInfoStr, out _newPlayerInfo);

	    if (_newPlayerInfo.guid == Network.player.guid)
	    {
            Debug.LogWarning("Trying to refresh self. Ignore.");
	        return;
	    }

	    PlayerInfo _player;
        if (!players.TryGetValue(_newPlayerInfo.guid, out _player)) 
		{
            _player = _newPlayerInfo;
            players.Add(_player.guid, _player);

            if (_player.connected)
		    {
                _player.connected = false;
		        Connect(_player);
		    }
		}
		else
		{
            Copy(_player, _newPlayerInfo);

            if (_player.connected != _newPlayerInfo.connected)
		    {
                if (_newPlayerInfo.connected)
		            Connect(_player);
		        else
		            Disconnect(_player);
		    }
		}

        if (postSetuped != null) postSetuped(_player);
	}
    
    [RPC]
    void PlayerManager_ResponseRefreshPlayerInfoDone()
    {
        m_IsRefreshingPlayerInfo = false;
        if (m_RefreshPlayerInfoCallback != null) m_RefreshPlayerInfoCallback();
        m_RefreshPlayerInfoCallback = null;
    }
        
    private bool m_IsUpdatingInfo = false;
    private Action m_UpdateInfoCallbacks;

	public void UpdateInfo(Action _callback)
	{
		if (Network.peerType == NetworkPeerType.Disconnected)
		{
            if (_callback != null) _callback();
		    return;
		}

        m_UpdateInfoCallbacks += _callback;
		if (m_IsUpdatingInfo) return;
        m_IsUpdatingInfo = true;
		var _playerInfoSerial = NetworkSerializer.Serialize(mine);
        networkView.RPC("PlayerManager_RequestUpdateInfo", RPCMode.Others, Network.player, _playerInfoSerial);

	    if (Network.isServer)
	        PlayerManager_ResponseUpdateInfo();
	}

	[RPC]
	void PlayerManager_RequestUpdateInfo(NetworkPlayer _requestor, string _playerInfoSerial)
	{
		PlayerInfo _player;
        NetworkSerializer.Deserialize(_playerInfoSerial, out _player);

        var _playerLocal = Get(_player.guid);

	    if (_playerLocal == null)
	    {
            _playerLocal = _player;
            _playerLocal.connected = false;
            Add(_playerLocal);
            Connect(_playerLocal);
	    }
	    else
	    {
            Copy(_playerLocal, _player);
            if (!_playerLocal.connected)
                Connect(_playerLocal);
        }

        if (postInfoChanged != null) postInfoChanged(_player);

	    if (Global.Server() == Network.player.guid)
            networkView.RPC("PlayerManager_ResponseUpdateInfo", _requestor);
	}

    [RPC]
    void PlayerManager_ResponseUpdateInfo()
    {
        m_IsUpdatingInfo = false;
        if (m_UpdateInfoCallbacks != null) m_UpdateInfoCallbacks();
        m_UpdateInfoCallbacks = null;
    }
}

