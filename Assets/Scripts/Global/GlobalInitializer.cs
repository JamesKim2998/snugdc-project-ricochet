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
			_global.bgm.Stop();
			_global.bgm.clip = bgm;
			_global.bgm.loop = true;
			_global.bgm.Play();
		}

        _global.transition.gameCutScene = gameCutScenePrf;

		if (destroyAfterInitialize)
			Destroy( gameObject);
	}
}
