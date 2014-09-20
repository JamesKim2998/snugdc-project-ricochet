using UnityEngine;
using System.Collections;

public class GlobalSFXAction : MonoBehaviour
{
    public AudioSource sfx;

    public void Play()
    {
        if (! Global.SFX().mute)
            sfx.Play();
    }
}
