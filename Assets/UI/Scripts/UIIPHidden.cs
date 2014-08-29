using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UILabel))]
public class UIIPHidden : MonoBehaviour {
	public UILabel inputLabel;
	public bool hidden = true;
	public string textHidden = "---.---.---.--- (Hidden)";

	void Start () 
	{
		if (hidden) 
			inputLabel.text = textHidden;
	}

	void OnClick() 
	{
		hidden = ! hidden;

		if (hidden) 
		{
			inputLabel.text =  textHidden;
		}
		else
		{
			inputLabel.text =  Network.player.ipAddress + " (Shown)";
		}
	}
}
