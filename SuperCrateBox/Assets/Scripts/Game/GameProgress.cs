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
		STOP,
	}

	private State m_State = State.STOP;
	public State state { get { return m_State; }}
	public bool IsState(State _state) { return m_State == _state; }

	private State m_NextState = State.NULL;

	public Action postStart;
	public Action postRun;
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

	void Update()
	{
		if (m_NextState !=  State.NULL)
		{
			var _nextState = m_NextState;
			m_NextState = State.NULL;

			switch (_nextState)
			{
			case State.START:   DoStartGame(); break;
			case State.RUNNING: DoRunGame(); break;
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
		
		networkView.RPC("GameProgress_StartGame", RPCMode.All);
	}
		
	[RPC]
	void GameProgress_StartGame()
	{
		Debug.Log("StartGame");
		m_State = State.START;
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
		m_State = State.RUNNING;
		if (postRun != null) postRun();
	}

	public void StopGame()
	{
		m_NextState = State.STOP;
	}

	void DoStopGame()
	{
		if (! IsStateChangable())
			return;

		if (state != State.RUNNING) 
		{
			Debug.LogWarning("Game only can be stopped on RUNNING state.");
			return;
		}
		
		networkView.RPC("GameProgress_StopGame", RPCMode.All);
	}
	
	[RPC]
	void GameProgress_StopGame()
	{
		Debug.Log("StopGame");
		m_State = State.STOP;
		if (postStop != null) postStop();
		// Game.Instance.Purge();
	}
}
