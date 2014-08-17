using UnityEngine;
using System.Collections;

public class GlobalInitializer : MonoBehaviour
{
	public bool bgmEnabled = true;
	public AudioClip bgm;

	void Start ()
	{
		var _global = Global.Instance;
		var _game = Game.Instance;

		if (bgmEnabled && bgm != null)
		{
			_global.bgm.Stop();
			_global.bgm.clip = bgm;
			_global.bgm.loop = true;
			_global.bgm.Play();
		}
	}
}

