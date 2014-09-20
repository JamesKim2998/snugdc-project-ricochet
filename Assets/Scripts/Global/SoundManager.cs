using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public AudioSource bgm;
    public AudioSource sfx;
    
    public void Awake()
    {
        bgm = gameObject.AddComponent<AudioSource>();
        sfx = gameObject.AddComponent<AudioSource>();
    }

    public void MuteBGM()
    {
        bgm.enabled = false;
        bgm.mute = true;
        if (!sfx.enabled)
            AudioListener.pause = true;
    }

    public void UnmuteBGM()
    {
        bgm.enabled = true;
        bgm.mute = false;
        AudioListener.pause = false;
    }

    public void MuteSFX()
    {
        sfx.enabled = false;
        sfx.mute = true;
        if (!bgm.enabled)
            AudioListener.pause = true;
    }

    public void UnmuteSFX()
    {
        sfx.enabled = true;
        sfx.mute = false;
        AudioListener.pause = false;
    }

}
