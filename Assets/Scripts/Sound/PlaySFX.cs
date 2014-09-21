using UnityEngine;
using System.Collections;

public class PlaySFX : MonoBehaviour
{
    public bool playOnStart = true;
    public bool deleteAfterPlay = true;

	void Start ()
	{
	    if (playOnStart)
            Play();
	}

    public void Play()
    {
        if (Global.Sound.isSFXEnabled)
            audio.Play();

        if (deleteAfterPlay)
            Destroy(this);
    }
}
