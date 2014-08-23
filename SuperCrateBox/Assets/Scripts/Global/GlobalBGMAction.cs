using UnityEngine;
using System.Collections;

public class GlobalBGMAction : MonoBehaviour 
{
	public void ExecuteEnable()
	{
		Global.BGM().enabled = true;
	}

	public void ExecuteDisable()
	{
		Global.BGM().enabled = false;
	}

	public void ExecuteToggle()
	{
		Global.BGM().enabled = ! Global.BGM().enabled;
	}

	public AudioClip paramBGM;
	public void ExecuteChangeBGM()
	{
		Global.BGM().clip = paramBGM;
		Global.BGM().Play();
	}
}
