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

	public bool m_IntroOnce = false;

    public float timeElapsed { get; private set; }

    public float stateTime { get; private set; }

    public int gameID { get; private set; }

    public Action postIntro;
	public Action postStart;
	public Action postRun;
	public Action postOver;
	public Action postStop;

    public GameProgress()
    {
        stateTime = 0f;
        timeElapsed = 0f;
    }

    static bool IsStateChangable()
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

	public bool TryIntroGame()
	{
		if (state != State.STOP)
		{
			Debug.LogWarning("Intro only can be started from STOP state. Ignore.");
			return false;
		}

		if (m_IntroOnce)
		{
			Debug.LogWarning("Intro already has been entered. Ignore.");
			return false;
		}

		m_IntroOnce = true;
		if (postIntro != null) postIntro();
		return true;
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
		gameID = _gameID;
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

	public bool CanStopGame()
	{
		// stop은 클라이언트에서도 호출할 수 있습니다.
		if (state != State.RUNNING && state != State.OVER) 
		{
//			Debug.LogWarning("Game only can be stopped on RUNNING or OVER state. But now in " + state.ToString() + " state.");
			return false;
		}
		
		return true;
	}

	public bool TryStopGame()
	{
		var _ret = CanStopGame();
		if (_ret) StopGame();
		return _ret;
	}

	void StopGame()
	{
		m_NextState = State.STOP;
	}

	void DoStopGame()
	{
		if (! CanStopGame())
		{
			Debug.LogError("Cannot stop game. Ignore. Now: " + state + ".");
			return;
		}

		state = State.STOP;
		m_IntroOnce = false;
		if (postStop != null) postStop();
		Game.Instance.Purge();
	}
}
