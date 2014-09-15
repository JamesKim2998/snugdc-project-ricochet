using UnityEngine;
using System.Collections;

public class GameLevel
{
	public GameLevel(LevelDef _def) 
	{
		characterSpawners = new CharacterSpawners(_def.characterSpawners);
        characterSpawners.postSpawn += ListenCharacterSpawned;
        characterSpawners.postDestroy += ListenCharacterDestroyed;

		Global.Server().postConnected += ListenConnectedToServer;
		Global.Server().postDisconnected += ListenDisconnectedFromServer;
	}

	~GameLevel()
    {
		Global.Server().postConnected -= ListenConnectedToServer;
		Global.Server().postDisconnected -= ListenDisconnectedFromServer;

        characterSpawners.postSpawn -= ListenCharacterSpawned;
        characterSpawners.postDestroy -= ListenCharacterDestroyed;
	}

	public CharacterSpawners characterSpawners;

	void ListenConnectedToServer()
    {
        characterSpawners.isEnabled = true;
	}

	void ListenDisconnectedFromServer()
	{
		characterSpawners.isEnabled = false;
	}

    static void ListenCharacterSpawned(CharacterSpawner _spawner, Character _character)
    {
        Game.Character.Add(_character);
    }

    static void ListenCharacterDestroyed(CharacterSpawner _spawner, Character _character)
    {
        // Game.Character.characters.Remove(_character.id);
    }
}

