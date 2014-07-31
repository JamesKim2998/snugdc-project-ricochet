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

public class TransitionManager : MonoBehaviour
{
	GameTransition m_GameTransition;

	public void RequestStartGame(GameTransition _transition)
	{
		if (! Network.isServer)
		{
			Debug.Log("Only server can start the game!");
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
	}
}

