using UnityEngine;
using System.Collections;

public class SFXAction : MonoBehaviour
{
    public AudioSource sfx;

    public void Play()
    {
        if (Global.Sound.isSFXEnabled)
            sfx.Play();
    }
}
