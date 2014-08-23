using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class GameProgress : MonoBehaviour
{
	public enum State
	{
		NULL,
		START,
		RUNNING,
		OVER,
		STOP,
	}

	private State m_State = State.STOP;
	public State state { get { return m_State; } private set { m_State = value; stateTime = 0; }}
	public bool IsState(State _state) { return state == _state; }

	private State m_NextState = State.NULL;

	private float m_TimeElapsed = 0f;
	public float timeElapsed { get { return m_TimeElapsed; } private set {m_TimeElapsed = value; } }
	
	private float m_StateTime = 0f;
	public float stateTime { get { return m_StateTime; } private set { m_StateTime = value; } }

	private int m_GameID;
	public int gameID { get { return m_GameID; } private set { m_GameID = value; } }

	public Action postStart;
	public Action postRun;
	public Action postOver;
	public Action postStop;

	bool IsStateChangable()
	{
		if (! Network.isServer)
		{
			Debug.LogWarning("Only server can change progress.");
			return false;
		}

		return true;
	}

	void Purge()
	{
		if (IsState(State.STOP))
		{
			Debug.LogError("Purge is only allowed for STOP state");
			return;
		}

		timeElapsed = 0;
		stateTime = 0;
	}

	void Update()
	{
		timeElapsed += Time.deltaTime;
		stateTime += Time.deltaTime;
		
		if (m_NextState !=  State.NULL)
		{
			var _nextState = m_NextState;
			m_NextState = State.NULL;

			switch (_nextState)
			{
			case State.START:   DoStartGame(); break;
			case State.RUNNING: DoRunGame(); break;
			case State.OVER:    DoOverGame(); break;
			case State.STOP:    DoStopGame(); break;
			}
		}
	}

	public void StartGame()
	{
		m_NextState = State.START;
	}

	private void DoStartGame()
	{
		if (! IsStateChangable())
			return;
		
		if (state != State.STOP) 
		{
			Debug.LogWarning("Game only can be started on STOP state.");
			return;
		}

		gameID = Global.Random ().Next ();
		networkView.RPC("GameProgress_StartGame", RPCMode.All, gameID);
	}
		
	[RPC]
	void GameProgress_StartGame(int _gameID)
	{
		Debug.Log("StartGame");
		state = State.START;
		timeElapsed = 0;
		if (postStart != null) postStart();
	}

	public void RunGame()
	{
		m_NextState = State.RUNNING;
	}

	void DoRunGame()
	{
		if (! IsStateChangable())
			return;
		
		if (state != State.START) 
		{
			Debug.LogWarning("Game only can be ran on START state.");
			return;
		}
		
		networkView.RPC("GameProgress_RunGame", RPCMode.All);
	}

	[RPC]
	void GameProgress_RunGame()
	{
		Debug.Log("RunGame");
		state = State.RUNNING;
		if (postRun != null) postRun();
	}

	public bool CanOverGame()
	{
		if (! IsStateChangable())
			return false;
		
		if (state != State.RUNNING) 
		{
//			Debug.LogWarning("Game only can be over on RUNNING state.");
			return false;
		}

		return true;
	}

	public bool TryOverGame()
	{
		var _ret = CanOverGame();
		if (_ret) OverGame();
		return _ret;
	}

	void OverGame()
	{
		m_NextState = State.OVER;
	}
	
	void DoOverGame()
	{
		if ( ! CanOverGame())
		{
			Debug.LogError("Cannot over game. Ignore.");
			return;
		}

		networkView.RPC("GameProgress_OverGame", RPCMode.All);
	}
	
	[RPC]
	void GameProgress_OverGame()
	{
		Debug.Log("OverGame");
		state = State.OVER;
		if (postOver != null) postOver();
	}

	public void StopGame()
	{
		m_NextState = State.STOP;
	}

	void DoStopGame()
	{
		// stop은 클라이언트에서도 호출할 수 있습니다.
		if (state != State.RUNNING && state != State.OVER) 
		{
			Debug.LogWarning("Game only can be stopped on RUNNING or OVER state. But now in " + state.ToString() + " state.");
			return;
		}
		
		Debug.Log("StopGame");
		state = State.STOP;
		if (postStop != null) postStop();
		Game.Instance.Purge();
	}
}
