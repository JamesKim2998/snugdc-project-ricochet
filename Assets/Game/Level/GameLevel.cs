using UnityEngine;
using System.Collections;

public class GameLevel
{
    private bool m_IsDisposed = false;

	public GameLevel(GameLevelDef _def) 
	{
		characterSpawners = new CharacterSpawners(_def.characterSpawners);
        characterSpawners.postSpawn += ListenCharacterSpawned;
        characterSpawners.postDestroy += ListenCharacterDestroyed;

		Global.Server().postConnected += ListenConnectedToServer;
		Global.Server().postDisconnected += ListenDisconnectedFromServer;
	}

    public void Dispose()
    {
        if (m_IsDisposed)
        {
            Debug.LogWarning("Trying to purge again! Ignore.");
            return;   
        }

        m_IsDisposed = true;

        Global.Server().postConnected -= ListenConnectedToServer;
        Global.Server().postDisconnected -= ListenDisconnectedFromServer;

        characterSpawners.postSpawn -= ListenCharacterSpawned;
        characterSpawners.postDestroy -= ListenCharacterDestroyed;
    }

	~GameLevel()
    {
        if (!m_IsDisposed) Dispose();
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

