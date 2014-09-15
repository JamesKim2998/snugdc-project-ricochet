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
	private int m_GameID = 0;
	private bool m_IsLatest = false;
	public bool isLatest { get { return (m_GameID == Game.Progress.gameID) && m_IsLatest; } }

	public Dictionary<string, PlayerResult> results = new Dictionary<string, PlayerResult>();
	public Action postPropagated;

	void Start()
	{
		Game.Progress.postStart += ListenGameStart;
		Game.Progress.postOver += ListenGameOver;
	}

	void OnDestroy()
	{
		Game.Progress.postStart -= ListenGameStart;
		Game.Progress.postOver -= ListenGameOver;
	}

	public void FillIn()
	{
		if (isLatest)
		{
			Debug.LogWarning("Trying to fill in again. Ignore.");
			return;
		}

		m_IsLatest = true;

		results = new Dictionary<string, PlayerResult> ();

		foreach (var _player in Global.Player().players)
		{
			var _result = new PlayerResult();
			var _statistic = Game.Statistic.Get(_player.Key);
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

		if (! isLatest )
			FillIn ();

		Game.Instance.networkView.RPC("GameResult_Propagate", RPCMode.All, m_GameID, NetworkSerializer.Serialize (results));
	}

	[RPC]
	private void GameResult_Propagate(int _gameID, string _resultsSerial) 
	{
		results = new Dictionary<string, PlayerResult> ();

		if (m_GameID != _gameID)
		{
			Debug.LogError("Received result of wrong game id. Ignore.");
			return;
		}

		NetworkSerializer.Deserialize (_resultsSerial, out results);

		if (postPropagated != null) postPropagated();
	}
	
	private void ListenGameStart()
	{
		m_GameID = Game.Progress.gameID;
		m_IsLatest = false;
	}

	private void ListenGameOver()
	{
		if (! Network.isServer)
			return;

		Propagate ();
	}
}

