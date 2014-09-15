using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDResultBoard : MonoBehaviour
{
	public UIGrid grid;
	public GameObject entityPrf;
	private Dictionary<string, HUDResultBoardEntity> m_Entities;
	
	void Start()
	{
		m_Entities = new Dictionary<string, HUDResultBoardEntity> ();
		if (Game.Progress.IsState(GameProgress.State.OVER)) Refresh();
		Game.Result.postPropagated +=  ListenResultPropagated;
	}

	void OnDestroy()
	{
		Game.Result.postPropagated -=  ListenResultPropagated;
	}

	void Refresh()
	{
		foreach ( var _result in Game.Result.results )
			Add(_result.Key, _result.Value);
		grid.Reposition();
	}

	void Add(string _player, PlayerResult _result)
	{
		if (m_Entities.ContainsKey(_player))
		{
			Debug.LogWarning("Trying to add already existing player " + _player + ".");
			return;
		}

		var _obj = Object.Instantiate(entityPrf) as GameObject;
		_obj.transform.parent = grid.transform;
		_obj.transform.localPosition = Vector3.zero;
		_obj.transform.localScale = Vector3.one;
		
		var _entity = _obj.GetComponent<HUDResultBoardEntity>();
		m_Entities[_player] = _entity;
		_entity.player = _player;

		_entity.player = _player;
		_entity.weaponPickUp = _result.weaponPickUp;
		_entity.kill = _result.kill;
		_entity.death = _result.death;
		_entity.score = _result.score;
	}

	void ListenResultPropagated()
	{
		Refresh ();
	}
}

