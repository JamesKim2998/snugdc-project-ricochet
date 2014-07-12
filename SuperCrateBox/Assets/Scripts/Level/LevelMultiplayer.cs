using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelMultiplayer : MonoBehaviour 
{
	private bool m_IsInitialized = false;
	public List<CharacterSpawner> characterSpawners;

	void Start () 
	{
		foreach(CharacterSpawner _spawner in characterSpawners)
		{
			_spawner.postDestroy += ListenCharacterDestroyed;
			_spawner.enabled = false;
			_spawner.autoSpawn = false;
			_spawner.networkView.enabled = true;
		}
			
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
		if (m_IsInitialized) return;
		m_IsInitialized = true;
		SpawnCharacter ();
	}

	void ListenBeforeDisconnected()
	{
		Debug.Log ("server disconnected.");
		foreach (var _spawner in characterSpawners) 
			_spawner.enabled = false;
		Network.RemoveRPCs(Network.player);
		Network.DestroyPlayerObjects(Network.player);
	}

	void OnPlayerDisconnected(NetworkPlayer player) {
		Debug.Log("Clean up after player " + player);
		Network.RemoveRPCs(player);
		Network.DestroyPlayerObjects(player);
	}

	void SpawnCharacter()
	{
		if (Game.Character ().character != null) 
		{
			Debug.Log("Trying to spawner character again! Ignore.");
			return;
		}

		var _spawnerCnt = Random.Range (0, characterSpawners.Count);
		var _spawnerTarget = characterSpawners[_spawnerCnt];
		var _character = _spawnerTarget.Spawn();	
		Game.Character().character = _character;
	}

	void ListenCharacterDestroyed(CharacterSpawner _spawner, GameObject _obj) 
	{
		SpawnCharacter ();

	}
}
