using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIToggle))]
public class UISFXToggleButton : MonoBehaviour
{
    public UIToggle toggle;
    private bool m_Inited = false;

    void Start()
    {
        toggle.value = Global.SFX().enabled;
        m_Inited = true;
    }

    public void OnValueChanged()
    {
        if (!m_Inited) return;

        if (toggle.value)
            Global.Sound.UnmuteSFX();
        else
            Global.Sound.MuteSFX();
    }
}

