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
            if (postChanged != null) postChanged(this);
        }
    }

    private CharacterType m_CharacterSelected = CharacterType.BLUE;
    public CharacterType characterSelected
    {
        get { return m_CharacterSelected; }
        set
        {
            if (m_CharacterSelected == value) return;
            m_CharacterSelected = value;
            if (postChanged != null) postChanged(this);
        }
    }

    [System.NonSerialized]
    public Action<PlayerInfo> postChanged;

    public PlayerInfo(string _guid)
    {
        guid = _guid;
        if (isMine)
            m_Name = PlayerPrefs.GetString("player_name", "team_ricochet");
    }
}
