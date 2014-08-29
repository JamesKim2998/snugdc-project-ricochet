using UnityEngine;
using System.Collections;

public class UIIPInput : MonoBehaviour {
//	public bool host;
	public UIInput input;

	void Start()
	{
		input.label.text = GlobalVariables.JOIN_IP;
	}

	public void OnSubmit()
	{
		GlobalVariables.JOIN_IP = input.value;
	}
}
