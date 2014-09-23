using System.Linq;
using UnityEngine;
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

		if (Network.peerType != NetworkPeerType.Disconnected)
		{
		    foreach (var _player in Global.Player().Where(_player => _player.Value.connected))
		        Add(_player.Key);
		}

		Global.Player().postConnected += ListenPlayerConnected;
	}

	void OnDestroy()
	{
		Global.Player().postConnected -= ListenPlayerConnected;
	}
		
	void Add(string _player)
	{
		if (Global.Server().server == _player )
		{
			Debug.LogWarning("Trying to add server.");
			return;
		}

		if (m_Entries.ContainsKey(_player)) 
		{
			Debug.LogWarning("Trying to add already existing player.");
			return;
		}

		var _obj = (GameObject) Instantiate(entryPrf, Vector3.zero, Quaternion.identity);
		var _scale = _obj.transform.localScale;
		_obj.transform.parent = transform;
		_obj.transform.localScale = _scale;
		m_Entries[_player] = _obj;
		
		var _entry = _obj.GetComponent<UIRoomPlayerEntry>();
		_entry.player = Global.Player().Get(_player);

		m_Grid.Reposition();
	}

	void Remove(string _player)
	{
		GameObject _entry;

		if (! m_Entries.TryGetValue(_player, out _entry)) 
		{
			Debug.LogWarning("Trying to remove not existing player.");
			return;
		}

		Destroy(_entry);

		m_Entries.Remove(_player);

		m_Grid.Reposition();
	}

	void ListenPlayerConnected(bool _connected, string _player) 
	{
		if (_connected) 
			Add(_player);
		else 
			Remove(_player);
	}

}
