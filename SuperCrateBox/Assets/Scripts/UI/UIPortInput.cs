using UnityEngine;
using System.Collections;

public class UIPortInput : MonoBehaviour {
	public bool host = true;
	public UIInput input;

	void Start()
	{
		if (host)
		{
			if (GlobalVariables.HOST_PORT.HasValue)
				input.label.text = GlobalVariables.HOST_PORT.ToString();
		}
		else
		{
			if (GlobalVariables.JOIN_PORT.HasValue)
				input.label.text = GlobalVariables.JOIN_PORT.ToString();
		}
	}

	public void OnSubmit()
	{
		int _port;
		if (int.TryParse (input.value, out _port))
		{
			if (host)
			{
				GlobalVariables.HOST_PORT = _port;
			}
			else
			{
				GlobalVariables.JOIN_PORT = _port;	
			}
		}
	}
}
