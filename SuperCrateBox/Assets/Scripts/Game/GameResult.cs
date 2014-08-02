using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerResult
{
	public CharacterType characterType = CharacterType.NONE;
	public int weaponPickUp = 0;
	public int kill = 0;
	public int death = 0;
	public int score = 0;
}

public class GameResult : MonoBehaviour
{
	public Game game;

	public int gameID = 0;
	public Dictionary<string, PlayerResult> results;

	public Action postPropagated;

	void Start()
	{
		results = new Dictionary<string, PlayerResult> ();
		Game.Progress ().postOver += ListenGameOver;
	}

	void OnDestroy()
	{
		Game.Progress ().postOver -= ListenGameOver;
	}

	public bool IsLatest()
	{
		return gameID == Game.Progress ().gameID;
	}

	public void FillIn()
	{
		if (IsLatest())
		{
			Debug.LogWarning("Trying to reset again. Ignore.");
			return;
		}

		gameID = Game.Progress().gameID;

		results = new Dictionary<string, PlayerResult> ();

		foreach (var _player in Global.Player().players)
		{
			var _result = new PlayerResult();
			var _statistic = Game.Statistic().Get(_player.Key);
//			_result.kill = ;
			_result.death = _statistic.death;
			_result.score = _statistic.score;
			results.Add(_player.Key, _result);
		}
	}

	public void Propagate()
	{
		if (! Network.isServer)
		{
			Debug.LogWarning("Only server can propagate results!");
			return;
		}

		if (! IsLatest ())
			FillIn ();

		game.networkView.RPC("GameResult_Propagate", RPCMode.All, gameID, NetworkSerializer.Serialize (results));
	}

	[RPC]
	private void GameResult_Propagate(int _gameID, string _resultsSerial) 
	{
		if (gameID != _gameID)
		{
			Debug.LogError("Received result of wrong game id. Ignore.");
			results = null;
			return;
		}

		results = new Dictionary<string, PlayerResult> ();
		NetworkSerializer.Deserialize (_resultsSerial, out results);

		if (postPropagated != null) postPropagated();
	}

	private void ListenGameOver()
	{
		if (! Network.isServer)
			return;

		Propagate ();
	}
}

