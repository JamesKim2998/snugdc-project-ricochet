using UnityEngine;
using System.Collections;

public class GameLevelDef : MonoBehaviour
{
	public bool applyOnStart = true;
	public bool autoremove = true;

	public CharacterSpawnersDef characterSpawners;

	void Start()
	{
		if (applyOnStart) Apply();
	}

    void OnDestroy()
    {
        if (autoremove && Game.Level != null)
        {
            Game.Level.Purge();
            Game.Level = null;
        }
    }

	void Apply()
	{
        Game.Level = new GameLevel(this);
	}
}
