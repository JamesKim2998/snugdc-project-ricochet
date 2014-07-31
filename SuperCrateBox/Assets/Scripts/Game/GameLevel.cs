using UnityEngine;
using System.Collections;

public class GameLevel
{
	public GameLevel(LevelDef _def) 
	{
		characterSpawners = new CharacterSpawners(_def.characterSpawners);

		Global.Server().postConnected += ListenConnectedToServer;
		Global.Server().postDisconnected += ListenDisconnectedFromServer;
	}

	~GameLevel()
	{
		Global.Server().postConnected -= ListenConnectedToServer;
		Global.Server().postDisconnected -= ListenDisconnectedFromServer;
	}

	public CharacterSpawners characterSpawners;
	
	void ListenConnectedToServer()
	{
		// cha
	}

	void ListenDisconnectedFromServer()
	{
		characterSpawners.isEnabled = false;
	}
}

