using UnityEngine;
using System.Collections;

[System.Serializable]
public class SceneTransition
{
	public ContextType context;
	public Scene scene;
}

[System.Serializable]
public class GameTransition
{
	public float setupDelay = 1f;

	public string map;
	public GameTransition Map(string _var) { map = _var; return this; }

	public GameModeDef mode;
	public GameTransition Mode(GameModeDef _var) { mode = _var; return this; }

	public void Setup()
	{
		Game.ModeManager().Setup(mode);
	}
}

public class TransitionManager : MonoBehaviour
{
	GameTransition m_GameTransition;

	public void RequestStartScene(SceneTransition _transition)
	{
		if (! Network.isServer)
		{
			Debug.Log("Only server is allowed to transfer to scene " + _transition.scene);
			return;
		}
		
		networkView.RPC("TransitionManager_RequestStartScene", RPCMode.All, NetworkSerializer.Serialize(_transition));
	}
	
	[RPC]
	void TransitionManager_RequestStartScene(string _transitionSerial)
	{
		var _transition = new SceneTransition();
		NetworkSerializer.Deserialize(_transitionSerial, out _transition);
		StartSceneLocal(_transition);
	}
	
	void StartSceneLocal(SceneTransition _transition)
	{
		LevelLoader.Instance.LoadLevel(SceneNames.Get(_transition.scene));
		Global.Context ().context = _transition.context;
	}

	// working locally
	public void StartLobby()
	{
		if (! Game.Progress().IsState(GameProgress.State.STOP))
		{
			if (! Game.Progress().TryStopGame())
			{
				Debug.LogError("Stoping game failed. Transfer abort.");
			}
		}
		LevelLoader.Instance.LoadLevel(SceneNames.Get(Scene.LOBBY));
		Global.Context().context = ContextType.LOBBY;
	}

	public void RequestStartGame(GameTransition _transition)
	{
		if (! Network.isServer)
		{
			Debug.Log("Only server can start the game!");
			return;
		}

		networkView.RPC("TransitionManager_RequestStartGame", RPCMode.All, NetworkSerializer.Serialize(_transition));
	}

	[RPC]
	void TransitionManager_RequestStartGame(string _transitionSerial)
	{
		var _transition = new GameTransition();
		NetworkSerializer.Deserialize(_transitionSerial, out _transition);
		StartGameLocal(_transition);
	}

	void StartGameLocal(GameTransition _transition)
	{
		LevelLoader.Instance.LoadLevel(_transition.map);
		if (_transition.setupDelay < 0) 
		{
			Debug.Log("Trying to setup GameMode without delay. Sure?");
			SetupGame();
		}
		else 
		{
			m_GameTransition = _transition;
			Invoke("SetupGame", m_GameTransition.setupDelay);
		}
	}

	void SetupGame()
	{
		m_GameTransition.Setup();
		m_GameTransition = null;
		Global.Context ().context = ContextType.GAME;
	}

}

