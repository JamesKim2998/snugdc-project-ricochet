using UnityEngine;
using System.Collections;

public class LevelMultiplayer : MonoBehaviour {

	public CharacterSpawner characterSpawner;

	void Start () {
		characterSpawner.enabled = false;
		characterSpawner.networkView.enabled = true;

		NetworkManager.postBeforeDisconnected += ListenBeforeDisconnected;
	}
	
	void Update () {
	
	}

	void OnServerInitialized() 
	{
		Debug.Log("server initialized.");
		InitLevelCommon();
	}

	void OnConnectedToServer() 
	{
		Debug.Log ("server connected.");
		InitLevelCommon();
	}

	void InitLevelCommon() 
	{
		characterSpawner.enabled = true;
	}

	void ListenBeforeDisconnected()
	{
		Debug.Log ("server disconnected.");
		characterSpawner.enabled = false;
		Network.RemoveRPCs(Network.player);
		Network.DestroyPlayerObjects(Network.player);
	}

	void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Clean up after player " + player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
}
