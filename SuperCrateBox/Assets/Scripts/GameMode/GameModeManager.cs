using UnityEngine;
using System.Collections;
using System;

public class GameModeManager : MonoBehaviour
{
	private GameMode m_Mode;
	public GameMode mode { get { return m_Mode; } } 
	private GameMode m_ModeToSet;
	
	public float setupDelay = 1f;
	public Action<GameMode> postChanged;

	public void Apply(GameModeManagerDef _def)
	{
		if (_def.mode != null)
		{
			if ( ! _def.mode.overrideMode)
			{
				if (m_Mode != null || m_ModeToSet != null)
				{
					Debug.Log("Mode is already exist. ");
				}
			}
			
			m_ModeToSet = _def.mode;

			if (setupDelay <= 0f) 
			{
				Debug.LogWarning("Setup mode without delay. Sure?");
				SetupProc();
			}
			else 
			{
				Invoke("SetupProc", setupDelay);
			}
		}
	}

	public void Purge()
	{
		Destroy(mode);
	}

	void SetMode(GameMode _mode)
	{
		if (m_Mode != null)
			Destroy(m_Mode);

		m_Mode = _mode;

		if (m_Mode) 
			m_Mode.Setup();
		
		if (postChanged != null)
			postChanged(m_Mode);
	}

	public void Setup(GameModeDef _def)
	{
		if (_def.mode == GameModeType.NULL)
		{
			Debug.LogError("Trying to setup NULL mode. Ignore.");
			return;
		}

		if ( ! _def.overrideMode)
		{
			if (mode != null || m_ModeToSet != null) 
			{
				Debug.Log("Mode is already exist. ");
				return;
			}
		}

		var _gameGO = Game.Instance.gameObject;
		
		switch (_def.mode)
		{
		case GameModeType.TEST: m_ModeToSet = _gameGO.AddComponent<GameModeTest>(); break;
		case GameModeType.DEATH_MATCH: m_ModeToSet = _gameGO.AddComponent<GameMode>(); break;
		default: Debug.LogError("Unknown game mode."); return;
		}

		m_ModeToSet.Init(_def);
		
		if (setupDelay <= 0f) 
		{
			Debug.LogWarning("Setup mode without delay. Sure?");
			SetupProc();
		}
		else 
		{
			Invoke("SetupProc", setupDelay);
		}
	}

	void SetupProc()
	{
		SetMode(m_ModeToSet);
	}

}

