using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIToggle))]
public class UIBGMToggleButton : MonoBehaviour
{
	public UIToggle toggle;
    private bool m_Inited = false;

	void Start ()
	{
        toggle.value = Global.Option.isBGMEnabled;
	    m_Inited = true;
	}

	public void OnValueChanged()
	{
        if (!m_Inited) return;
	    Global.Option.isBGMEnabled = toggle.value;
	}
}

