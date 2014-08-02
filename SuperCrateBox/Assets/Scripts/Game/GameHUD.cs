using UnityEngine;
using System.Collections;

// incomplete code
public class GameHUD 
{
	public UIRoot hudLayer;

	public GameObject chatscreen;
	public KeyCode chatscreenActivateKey = KeyCode.Return;
	public KeyCode chatscreenDeactivateKey = KeyCode.Escape;
	
	public HUDScoreBoard scoreBoard;
	public KeyCode scoreBoardActivateKey = KeyCode.Tab;
	
	public GameObject resultBoardParent;
	public GameObject resultBoardPrf;
	public HUDResultBoard resultBoard;
	
	public void Start()
	{
		Game.Progress ().postOver += ListenGameOver;
		Game.Progress ().postStop += ListenGameStop;
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

	~GameHUD()
	{
		Game.Progress ().postOver -= ListenGameOver;
		Game.Progress ().postStop -= ListenGameStop;
	}

	public void Update () 
	{
		// chatscreen activation.
		if (chatscreen != null) 
		{
			if (Input.GetKeyDown(chatscreenActivateKey))
				chatscreen.SetActive(true);

			if (Input.GetKeyDown(chatscreenDeactivateKey))
				chatscreen.SetActive(false);
		}
		
		// scoreboard activation.
		if (scoreBoard != null)
		{
			bool? _scoreBoardActivatePressed = null;
			
			if (Input.GetKeyDown(scoreBoardActivateKey))
			{
				_scoreBoardActivatePressed = true;
			}
			else if (Input.GetKeyUp(scoreBoardActivateKey))
			{
				_scoreBoardActivatePressed = false;
			}
			
			if (_scoreBoardActivatePressed != null) 
				scoreBoard.gameObject.SetActive(_scoreBoardActivatePressed.Value);
		}
	}

	void ListenGameOver()
	{
		if (resultBoard != null)
		{
			Debug.LogWarning("Result board is existing alreay. Why?");
			GameObject.Destroy(resultBoard);
		}

		if (resultBoardPrf != null)
		{
			var _obj = GameObject.Instantiate(resultBoardPrf, Vector3.zero, Quaternion.identity) as GameObject;
			resultBoard = _obj.GetComponent<HUDResultBoard>();

			GameObject _parent = null;

			if (resultBoardParent != null)
			{
				_parent = resultBoardParent;	
			}
			else 
			{
				if (hudLayer == null)
				{
					Debug.Log("No hud layer exist!");
				}
				else 
				{
					_parent = hudLayer.gameObject;
				}
			}

			if (_parent != null)
			{
				var _scale = _obj.transform.localScale;
				_obj.transform.parent = _parent.transform;
				_obj.transform.localPosition = Vector3.zero;
				_obj.transform.localScale = _scale;
			}
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
