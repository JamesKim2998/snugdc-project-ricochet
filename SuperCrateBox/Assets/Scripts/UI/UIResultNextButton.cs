using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIButton))]
public class UIResultNextButton : MonoBehaviour
{
	public UIButton button;

	void Start()
	{
		if (button == null) 
			button = GetComponent<UIButton>();

		button.onClick.Add(new EventDelegate(this, "Transfer"));
	}

	public void Transfer()
	{
		Global.Transition().StartLobby();
	}
}

