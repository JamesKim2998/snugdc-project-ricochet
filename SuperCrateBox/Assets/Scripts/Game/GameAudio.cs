using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameAudio 
{
	public bool pauseExpose = false;
	public float volumeExpose = 1;

	public void Start()
	{
		AudioListener.pause = pauseExpose;
		AudioListener.volume = volumeExpose;
	}
}

