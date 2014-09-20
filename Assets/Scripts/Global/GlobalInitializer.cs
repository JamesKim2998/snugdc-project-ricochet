using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalInitializer : MonoBehaviour
{
	public bool initializeOnStart = true;
	public bool destroyAfterInitialize = true;

	public AudioClip bgm;
    public GameObject gameCutScenePrf;

	void Start ()
	{
		if (initializeOnStart)
			Initialize();
	}

	void Initialize() 
	{
		var _global = Global.Instance;

		if (bgm != null)
		{
			Global.BGM().Stop();
            Global.BGM().clip = bgm;
            Global.BGM().loop = true;
            Global.BGM().Play();
		}

        _global.transition.gameCutScene = gameCutScenePrf;

		if (destroyAfterInitialize)
			Destroy( gameObject);
	}
}
