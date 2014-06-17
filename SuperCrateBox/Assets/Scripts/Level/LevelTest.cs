using UnityEngine;
using System.Collections;

public class LevelTest : MonoBehaviour {

	public ShooterSpawner shooterSpawner;

	void Start () {
		shooterSpawner.enabled = false;
		shooterSpawner.networkView.enabled = true;

		NetworkManager.postBeforeDisconnected += ListenBeforeDisconnected;
	}
	
	void Update () {
	
	}

	void OnServerInitialized() {
		Debug.Log("server initialized.");
		InitLevelCommon();
	}

	void OnConnectedToServer() {
		Debug.Log ("server connected.");
		InitLevelCommon();
	}

	void InitLevelCommon() 
	{
		shooterSpawner.enabled = true;
	}

	void ListenBeforeDisconnected()
	{
		Debug.Log ("server disconnected.");
		shooterSpawner.enabled = false;
		Network.RemoveRPCs(Network.player);
		Network.DestroyPlayerObjects(Network.player);
	}

	void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Clean up after player " + player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}
}
