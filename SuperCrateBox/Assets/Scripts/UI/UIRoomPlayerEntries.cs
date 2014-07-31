using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(UIGrid))]
public class UIRoomPlayerEntries : MonoBehaviour 
{
	public GameObject entryPrf;

	private UIGrid m_Grid;

	private Dictionary<string, GameObject> m_Entries;

	void Start() 
	{
		m_Entries = new Dictionary<string, GameObject>();

		m_Grid = GetComponent<UIGrid>();

		Add(Global.Player().mine);

		Global.Player().postConnected += ListenPlayerConnected;
		Global.Ready().postPoll += ListenPollReadyInfo;
	}

	void OnDestroy()
	{
		Global.Player().postConnected -= ListenPlayerConnected;
		Global.Ready().postPoll -= ListenPollReadyInfo;
	}
		
	void Add(PlayerInfo _player)
	{
		if (m_Entries.ContainsKey(_player.guid)) 
		{
			Debug.LogError("Trying to add already existing player.");
			return;
		}

		var _obj = GameObject.Instantiate(entryPrf, Vector3.zero, Quaternion.identity) as GameObject;
		var _scale = _obj.transform.localScale;
		_obj.transform.parent = transform;
		_obj.transform.localScale = _scale;
		m_Entries[_player.guid] = _obj;
		
		var _entry = _obj.GetComponent<UIRoomPlayerEntry>();
		_entry.player = _player;

		m_Grid.Reposition();
	}

	void Remove(PlayerInfo _player)
	{
		var _entry = m_Entries[_player.guid];

		if (! _entry) 
		{
			Debug.LogError("Trying to remove not existing player.");
			return;
		}

		GameObject.Destroy(_entry);

		m_Entries.Remove(_player.guid);

		m_Grid.Reposition();
	}

	void ListenPlayerConnected(PlayerInfo _player, bool _connected) 
	{
		if (_connected) 
		{
			Add(_player);
		}
		else 
		{
			Remove(_player);
		}
	}

	void ListenPollReadyInfo()
	{
		// var _newPlayers = new Dictionary<>;
	}
}
