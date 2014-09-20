using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIButton))]
public class UIResultNextButton : MonoBehaviour
{
	public UIButton button;

	void Start()
	{
		button.onClick.Add(new EventDelegate(this, "Transfer"));
	}

	public void Transfer()
	{
		Global.Transition().StartLobby();
	}
}

