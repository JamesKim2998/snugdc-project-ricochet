using UnityEngine;
using System.Collections;

public class HUDResultNextButton : MonoBehaviour 
{
	public string lobbyScene = SceneNames.MAIN_MENU;

	void Start()
	{
		Game.Progress ().postStop += ListenGameStop;
	}

	void OnDestroy()
	{
		Game.Progress ().postStop -= ListenGameStop;
	}

	public void OnSubmit()
	{
		Game.Progress ().StopGame ();
	}

	public void ListenGameStop()
	{
		var _transition = new SceneTransition ();
		_transition.context = ContextType.LOBBY;
		_transition.scene = lobbyScene;
		Global.Transition ().RequestStartScene (_transition);
	}
}
