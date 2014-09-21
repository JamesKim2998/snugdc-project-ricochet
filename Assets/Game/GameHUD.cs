using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameHUD : MonoBehaviour
{
	private GameHUDDef m_Def;

    private static HUDLayer s_MainLayer;
    public static HUDLayer mainLayer
    {
        get { return s_MainLayer; }
        set
        {
            if (mainLayer == value)
                return;

            if (s_MainLayer != null)
            {
                Debug.LogWarning("HUD main layer does already exist! Ignore.");
                return;
            }

            s_MainLayer = value;
        }
    }

	public GameObject chatscreen;
	public HUDScoreBoard scoreBoard;

    public HUDResultBoard resultBoard { get; private set; }

    private GameObject m_ModeHUD;

    void Awake()
    {
    }

	void Start()
	{
		Game.Progress.postIntro += ListenGameIntro;
		Game.Progress.postOver += ListenGameOver;
		Game.Progress.postStop += ListenGameStop;
		Game.ModeManager.postChanged += ListenModeChanged;
	}

	~GameHUD()
	{
		Game.Progress.postIntro -= ListenGameIntro;
		Game.Progress.postOver -= ListenGameOver;
		Game.Progress.postStop -= ListenGameStop;
		Game.ModeManager.postChanged -= ListenModeChanged;
	}
	
	public void Apply(GameHUDDef _def)
	{
		m_Def = _def;
	}
	
	public void Purge()
	{
		if (chatscreen != null) 
		{
			Destroy(chatscreen);
			chatscreen = null;
		}
		
		if (scoreBoard != null)
		{
            Destroy(scoreBoard);
			scoreBoard = null;
		}
		
		if (resultBoard != null)
		{
			Destroy(resultBoard);
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
		SetMode(_mode.type);
	}

	void ListenGameIntro()
	{
	}

	void ListenGameOver()
	{
		if (resultBoard != null)
		{
			Debug.LogWarning("Result board is existing already. Why?");
			Destroy(resultBoard);
		}

		if (m_Def.resultBoardPrf != null)
		{
			var _obj = (GameObject) Instantiate(m_Def.resultBoardPrf, m_Def.resultBoardPosition, Quaternion.identity);
			resultBoard = _obj.GetComponent<HUDResultBoard>();
			TransformHelper.SetParentLocal(resultBoard.gameObject, m_Def.hudRoot.gameObject);
		}
	}

	void ListenGameStop()
	{
		if (resultBoard != null)
			Destroy(resultBoard.gameObject);
	}

}
