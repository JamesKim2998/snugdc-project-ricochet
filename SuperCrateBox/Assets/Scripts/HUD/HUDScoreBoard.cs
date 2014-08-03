using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDScoreBoard : MonoBehaviour 
{
	public UIGrid grid;
	public GameObject entityPrf;

	Dictionary<string, HUDScoreBoardEntity> m_Entities;

	void Start () 
	{
		m_Entities = new Dictionary<string, HUDScoreBoardEntity>();

		foreach (var _player in Global.Player().players)
			Add(_player.Key);

		Global.Player().postConnected += ListenPlayerConnected;
	}

	void OnDestroy()
	{
		Global.Player().postConnected -= ListenPlayerConnected;
	}

	void Add(string _player)
	{
		if (m_Entities.ContainsKey(_player))
		{
			Debug.LogWarning("Trying to add already existing player " + _player + ".");
			return;
		}

		var _obj = GameObject.Instantiate(entityPrf, Vector3.zero, Quaternion.identity) as GameObject;
		var _scale = _obj.transform.localScale;
		_obj.transform.parent = transform;
		_obj.transform.localScale = _scale;
		
		var _entity = _obj.GetComponent<HUDScoreBoardEntity>();
		m_Entities[_player] = _entity;
		_entity.player = _player;
		
		grid.Reposition();
	}

	void ListenPlayerConnected(PlayerInfo _player, bool _connected)
	{
		if (_connected)
			Add (_player.guid);
	}

}
