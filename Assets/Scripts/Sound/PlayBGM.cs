using UnityEngine;
using System.Collections;

public class PlayBGM : MonoBehaviour
{
    public bool playOnStart = true;
    public bool deleteAfterPlay = true;

    void Start()
    {
        if (playOnStart)
            Play();
    }

    public void Play()
    {
        if (Global.Sound.isBGMEnabled)
            Global.Sound.PlayBGM(audio.clip);

        if (deleteAfterPlay)
            Destroy(this);
    }
}
