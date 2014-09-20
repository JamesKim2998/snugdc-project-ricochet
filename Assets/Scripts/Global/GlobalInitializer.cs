using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalInitializer : MonoBehaviour
{
	public bool initializeOnStart = true;
	public bool destroyAfterInitialize = true;

    public GameObject gameCutScenePrf;

	void Start ()
	{
		if (initializeOnStart)
			Initialize();
	}

	void Initialize() 
	{
		var _global = Global.Instance;

        _global.transition.gameCutScene = gameCutScenePrf;

		if (destroyAfterInitialize)
			Destroy( gameObject);
	}
}
