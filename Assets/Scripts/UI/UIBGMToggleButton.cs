using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIToggle))]
public class UIBGMToggleButton : MonoBehaviour
{
	private UIToggle m_Toggle;

	void Start ()
	{
		ComponentHelper.AssignComponentIfNotExists(gameObject, ref m_Toggle);
		m_Toggle.value = Global.BGM().enabled;
		m_Toggle.onChange.Add(new EventDelegate(this, "OnValueChanged"));
	}

	public void OnValueChanged()
	{
		Global.BGM().enabled = m_Toggle.value;
	}
}

