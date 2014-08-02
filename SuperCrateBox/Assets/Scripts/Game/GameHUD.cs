using UnityEngine;
using System.Collections;

// incomplete code
public class GameHUD 
{
	public GameObject chatscreen;
	public KeyCode chatscreenActivateKey = KeyCode.Return;
	public KeyCode chatscreenDeactivateKey = KeyCode.Escape;
	
	public HUDScoreBoard scoreBoard;
	public KeyCode scoreBoardActivateKey = KeyCode.Tab;

	void Start () {
	
	}
	
	public void Update () {
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
}
