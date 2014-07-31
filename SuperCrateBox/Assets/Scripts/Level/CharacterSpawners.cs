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
			
			foreach(CharacterSpawner _spawner in m_Spawners)
			{
				_spawner.enabled = false;
				_spawner.networkView.enabled = true;
				_spawner.postDestroy += ListenCharacterDestroyed;
			}
		}
	}
	
	private List<CharacterSpawner> m_Spawners;
	public List<CharacterSpawner> spawners { get { return m_Spawners; } }

	public Action<CharacterSpawner, Character> postDestroy;
	
	public CharacterSpawners(CharacterSpawnersDef _def) {
		m_Spawners = new List<CharacterSpawner>(_def.spawners);

		foreach(CharacterSpawner _spawner in m_Spawners)
		{
			_spawner.enabled = false;
			_spawner.networkView.enabled = true;
			_spawner.postDestroy += ListenCharacterDestroyed;
		}
	}

	public Character Spawn()
	{
#if DEBUG
		if (Game.Character().character != null)
			Debug.LogWarning("Trying to spawn a character, but already exist. ");
#endif

		if (m_Spawners.Count == 0)
		{
			Debug.LogError("No spawner is registered.");
			return null;
		}
			
		var _spawnerIdx = UnityEngine.Random.Range (0, m_Spawners.Count);
		var _spawnerTarget = m_Spawners[_spawnerIdx];
		var _character = _spawnerTarget.Spawn();	
		return _character;
	}

	void ListenCharacterDestroyed(CharacterSpawner _spawner, Character _character)
	{
		if (postDestroy != null) postDestroy(_spawner, _character);
	}
}

