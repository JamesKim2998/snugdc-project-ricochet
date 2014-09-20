using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    public AudioSource bgm;
    public AudioSource sfx;
    
    public void Awake()
    {
        bgm = gameObject.AddComponent<AudioSource>();
        bgm.loop = true;
        sfx = gameObject.AddComponent<AudioSource>();
    }

    public bool isBGMEnabled
    {
        get { return bgm.enabled && !bgm.mute; }
        set
        {
            if (isBGMEnabled == value)
                return;
            bgm.enabled = value;
            bgm.mute = !value;
            AudioListener.pause = !value && !sfx.enabled;
        }
    }

    public bool isSFXEnabled
    {
        get { return sfx.enabled && !sfx.mute; }
        set
        {
            if (isSFXEnabled == value)
                return;
            sfx.enabled = value;
            sfx.mute = !value;
            AudioListener.pause = !value && !bgm.enabled;
        }
    }

    public void PlayBGM(AudioClip _clip)
    {
        if (isBGMEnabled)
        {
            bgm.clip = _clip;
            bgm.Play();
        }
    }

    public void PlaySFX(AudioClip _clip)
    {
        if (isSFXEnabled)
            sfx.PlayOneShot(_clip);
    }
}
