using UnityEngine;
using System.Collections;

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
		GameMode.Setup(mode);
	}
}

[System.Serializable]
public class LobbyTransition
{
	public string scene;
	public LobbyTransition Scene(string _var) { scene = _var; return this; }
}

public class TransitionManager : MonoBehaviour
{
	GameTransition m_GameTransition;
	LobbyTransition m_LobbyTransition;

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
		Application.LoadLevel(_transition.map);

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

	public void RequestStartLobby(LobbyTransition _transition)
	{
		if (! Network.isServer)
		{
			Debug.Log("Only server is allowed to transfer to Lobby");
			return;
		}

		networkView.RPC("TransitionManager_RequestStartLobby", RPCMode.All, NetworkSerializer.Serialize(_transition));
	}

	[RPC]
	void TransitionManager_RequestStartLobby(string _transitionSerial)
	{
		var _transition = new LobbyTransition();
		NetworkSerializer.Deserialize(_transitionSerial, out _transition);
		StartLobbyLocal(_transition);
	}
	
	void StartLobbyLocal(LobbyTransition _transition)
	{
		Application.LoadLevel(_transition.scene);
		Global.Context ().context = ContextType.LOBBY;
	}
}

