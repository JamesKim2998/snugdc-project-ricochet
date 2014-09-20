using UnityEngine;
using System.Collections;

public class OptionManager : MonoBehaviour
{
    private bool m_Inited = false;

    void Start()
    {
        Global.Sound.isBGMEnabled = isBGMEnabled;
        Global.Sound.isSFXEnabled = isSFXEnabled;
        m_Inited = true;
    }

    public bool isBGMEnabled
    {
        get { return PlayerPrefs.GetInt("sound_bgm_enabled", 1) == 1; }
        set
        {
            PlayerPrefs.SetInt("sound_bgm_enabled", value ? 1 : 0);
            if (m_Inited) Global.Sound.isBGMEnabled = value;
        }
    }

    public bool isSFXEnabled
    {
        get { return PlayerPrefs.GetInt("sound_sfx_enabled", 1) == 1; }
        set
        {
            PlayerPrefs.SetInt("sound_sfx_enabled", value ? 1 : 0);
            if (m_Inited) Global.Sound.isSFXEnabled = value;
        }
    }

}
