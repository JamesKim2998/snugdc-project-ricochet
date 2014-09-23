using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class CharacterSpawners
{
	private bool m_IsEnabled = false;
	public bool isEnabled { 
		get { return m_IsEnabled; }
		set 
		{
			if (m_IsEnabled == value) return;
		    m_IsEnabled = value;

			foreach(var _spawner in spawners)
                _spawner.networkView.enabled = m_IsEnabled;
		}
	}
    public List<CharacterSpawner> spawners { get; private set; }

    public Action<CharacterSpawner, Character> postSpawn;
    public Action<CharacterSpawner, Character> postDestroy;
	
	public CharacterSpawners(CharacterSpawnersDef _def) {
		spawners = new List<CharacterSpawner>(_def.spawners);

		foreach(var _spawner in spawners)
		{
		    _spawner.postSpawn += ListenSpawn;
            _spawner.postDestroy += ListenDestroy;
		}
	}

    void Destroy()
    {
        foreach (var _spawner in spawners)
        {
            _spawner.postSpawn -= ListenSpawn;
            _spawner.postDestroy -= ListenDestroy;
        }
    }

	public Character Spawn()
	{
		if (Game.Character.character != null)
			Debug.LogWarning("Trying to spawn a character, but already exist. ");

		if (spawners.Count == 0)
		{
			Debug.LogError("No spawner is registered.");
			return null;
		}
			
		var _spawnerIdx = UnityEngine.Random.Range (0, spawners.Count);
		var _spawnerTarget = spawners[_spawnerIdx];
		var _character = _spawnerTarget.Spawn();	
		return _character;
	}

    void ListenSpawn(CharacterSpawner _spawner, Character _character)
    {
        if (postSpawn != null) postSpawn(_spawner, _character);
    }

    void ListenDestroy(CharacterSpawner _spawner, Character _character)
    {
        if (postDestroy != null) postDestroy(_spawner, _character);
    }


}

