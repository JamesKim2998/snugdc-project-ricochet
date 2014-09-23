using UnityEngine;
using System.Collections;

public class LevelDef : MonoBehaviour
{
	public bool applyOnStart = true;
	public bool destroyAfterApply = true;
	public CharacterSpawnersDef characterSpawners;

	void Start()
	{
		if (applyOnStart) Apply();
	}

	void Apply()
	{
		Game.InitLevel(this);
		if (destroyAfterApply) Destroy(gameObject);
	}
}
