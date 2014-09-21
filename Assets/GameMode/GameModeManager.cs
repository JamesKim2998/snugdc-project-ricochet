using UnityEngine;
using System.Collections;
using System;

public class GameModeManager : MonoBehaviour
{
    public GameMode mode { get; private set; }
    private GameMode m_ModeToSet;
	
	public float setupDelay = 1f;
	public Action<GameMode> postChanged;

	public void Apply(GameModeManagerDef _def)
	{
		if (_def.mode != null)
		{
			if ( ! _def.mode.overrideMode)
			{
				if (mode != null || m_ModeToSet != null)
				{
					Debug.Log("Mode is already exist. ");
				}
			}
			
			m_ModeToSet = _def.mode;
			
			CancelInvoke("SetupProc");

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
		mode = null;
	}

	void SetMode(GameMode _mode)
	{
		if (mode != null)
			Destroy(mode);

		mode = _mode;
		m_ModeToSet = null;

		if (mode) 
			mode.Setup();
		
		if (postChanged != null)
			postChanged(mode);
	}

	public void Setup(GameModeDef _def)
	{
		if (_def.type == GameModeType.NULL)
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
		
		switch (_def.type)
		{
		case GameModeType.TEST: m_ModeToSet = _gameGO.AddComponent<GameModeTest>(); break;
		case GameModeType.DEATH_MATCH: m_ModeToSet = _gameGO.AddComponent<GameModeDeathMatch>(); break;
		default: Debug.LogError("Unknown game mode."); return;
		}

		m_ModeToSet.Init(_def);
		
		CancelInvoke("SetupProc");

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
		m_ModeToSet = null;
	}

}

