using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class PlayerResult
{
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

    public PlayerResult mine
    {
        get
        {
            if (results.ContainsKey(Network.player.guid))
            {
                return results[Network.player.guid];
            }
            else
            {
                Debug.LogWarning("Game result doesn't contains mine. Return null.");
                return null;
            }
        }
    }

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
			var _statistic = Game.Statistic[_player.Key];
			_result.kill = _statistic.kill;
			_result.death = _statistic.death;
            _result.weaponPickUp = _statistic.weaponPickUp;
            _statistic.CalculateScore();
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

        foreach (var _result in results.Where(_result => ! CheckResult(_result.Key, _result.Value)))
            Debug.LogError("Result of player " + _result.Key + " is not valid. Continue anyway.");

		if (postPropagated != null) postPropagated();
	}

    public bool CheckResult(string _player, PlayerResult _result)
    {
        var _valid = true;
        var _playerInfo = Global.Player()[_player];
        var _statistic = Game.Statistic[_player];

        if (_statistic.weaponPickUp != _result.weaponPickUp)
        {
            Debug.LogError(_playerInfo.name + ": WeaponPickup does not match.");
            _valid = false;
        }

        if (_statistic.kill != _result.kill)
        {
            Debug.LogError(_playerInfo.name + ": Kill does not match.");
            _valid = false;
        }

        if (_statistic.death != _result.death)
        {
            Debug.LogError(_playerInfo.name + ": Death does not match.");
            _valid = false;
        }

        _statistic.CalculateScore();
        if (_statistic.score != _result.score)
        {
            Debug.LogError(_playerInfo.name + ": Score does not match.");
            _valid = false;
        }

        return _valid;
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

