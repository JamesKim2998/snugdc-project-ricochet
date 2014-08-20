using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameHUD : MonoBehaviour
{
	private GameHUDDef m_Def;

	public GameObject chatscreen;
	public HUDScoreBoard scoreBoard;
	
	private HUDResultBoard m_ResultBoard;
	public HUDResultBoard resultBoard { 
		get { return m_ResultBoard; } 
		private set { m_ResultBoard = value; } 
	}

	private GameObject m_ModeHUD;

	void Start()
	{
		Game.Progress ().postOver += ListenGameOver;
		Game.Progress ().postStop += ListenGameStop;
		Game.ModeManager().postChanged += ListenModeChanged;
	}

	~GameHUD()
	{
		Game.Progress ().postOver -= ListenGameOver;
		Game.Progress ().postStop -= ListenGameStop;
		Game.ModeManager().postChanged -= ListenModeChanged;
	}
	
	public void Apply(GameHUDDef _def)
	{
		m_Def = _def;
	}
	
	public void Purge()
	{
		if (chatscreen != null) 
		{
			GameObject.Destroy(chatscreen);
			chatscreen = null;
		}
		
		if (scoreBoard != null)
		{
			GameObject.Destroy(scoreBoard);
			scoreBoard = null;
		}
		
		if (resultBoard != null)
		{
			GameObject.Destroy(resultBoard);
			resultBoard = null;
		}
	}

	void Update () 
	{
		// chatscreen activation.
		if (chatscreen != null && m_Def.useChatscreenKey) 
		{
			if (Input.GetKeyDown(m_Def.chatscreenActivateKey))
				chatscreen.SetActive(true);

			if (Input.GetKeyDown(m_Def.chatscreenDeactivateKey))
				chatscreen.SetActive(false);
		}
		
		// scoreboard activation.
		if (scoreBoard != null && m_Def.useScoreBoardActivateKey)
		{
			bool? _scoreBoardActivatePressed = null;
			
			if (Input.GetKeyDown(m_Def.scoreBoardActivateKey))
			{
				_scoreBoardActivatePressed = true;
			}
			else if (Input.GetKeyUp(m_Def.scoreBoardActivateKey))
			{
				_scoreBoardActivatePressed = false;
			}
			
			if (_scoreBoardActivatePressed != null) 
				scoreBoard.gameObject.SetActive(_scoreBoardActivatePressed.Value);
		}
	}

	public void SetMode(GameModeType _mode)
	{
		if (m_ModeHUD != null)
		{
			Destroy(m_ModeHUD);
			m_ModeHUD = null;
		}

		var _prefab = m_Def.modeHUDs.Find(_modeHUD => _modeHUD.mode == _mode).prefab;

		if (_prefab == null) 
		{
			Debug.LogWarning("HUD for mode " + _mode + " not found.");
			return;
		}

		m_ModeHUD = Instantiate(_prefab, Vector3.zero, Quaternion.identity) as GameObject;
		TransformHelper.SetParentLocal(m_ModeHUD, m_Def.hudRoot.gameObject);
	}

	void ListenModeChanged(GameMode _mode )
	{
		SetMode(_mode.mode);
	}

	void ListenGameOver()
	{
		if (resultBoard != null)
		{
			Debug.LogWarning("Result board is existing alreay. Why?");
			GameObject.Destroy(resultBoard);
		}

		if (m_Def.resultBoardPrf != null)
		{
			var _obj = GameObject.Instantiate(m_Def.resultBoardPrf, Vector3.zero, Quaternion.identity) as GameObject;
			resultBoard = _obj.GetComponent<HUDResultBoard>();
			TransformHelper.SetParentWithoutScale(resultBoard.gameObject, m_Def.hudRoot.gameObject);
		}
	}

	void ListenGameStop()
	{
		if (resultBoard != null)
		{
			GameObject.Destroy(resultBoard.gameObject);
		}
	}

}
