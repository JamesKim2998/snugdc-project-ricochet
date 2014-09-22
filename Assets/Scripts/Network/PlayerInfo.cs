using System;
using UnityEngine;
using System.Collections;

[System.Serializable]
public class PlayerInfo
{
    public string guid { get; private set; }

    public bool connected = false;

    public bool isMine { get { return Network.player.guid == guid; } }

    private string m_Name = "undefined";
    public string name
    {
        get { return m_Name; }
        set
        {
            if (m_Name == value) return;
            m_Name = value;
            if (isMine) PlayerPrefs.SetString("player_name", m_Name);
            if (postNameChanged != null) postNameChanged(this);
        }
    }

    public readonly ObservableValue<bool> isReady
        = new ObservableValue<bool>(false);

    public readonly ObservableValue<CharacterType> characterSelected
        = new ObservableValue<CharacterType>(CharacterType.BLUE);

    [NonSerialized]
    public Action<PlayerInfo> postNameChanged;

    [NonSerialized]
    public Action<PlayerInfo> postChanged;

    public PlayerInfo(string _guid)
    {
        guid = _guid;

        if (isMine)
            m_Name = PlayerPrefs.GetString("player_name", "team_ricochet");

        postNameChanged += delegate { if (postChanged != null) postChanged(this); };
        isReady.postChanged += delegate { if (postChanged != null) postChanged(this); };
        characterSelected.postChanged += delegate { if (postChanged != null) postChanged(this); };
    }

    public static implicit operator string(PlayerInfo _playerInfo)
    {
        return _playerInfo == null ? null : _playerInfo.guid;
    }
}
