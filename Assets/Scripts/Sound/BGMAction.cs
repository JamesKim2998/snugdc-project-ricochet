using UnityEngine;
using System.Collections;

public class BGMAction : MonoBehaviour 
{
	public void ExecuteEnable()
	{
		Global.Sound.isBGMEnabled = true;
	}

	public void ExecuteDisable()
	{
		Global.Sound.isBGMEnabled = false;
	}

	public void ExecuteToggle()
	{
		Global.Sound.isBGMEnabled = !Global.Sound.isBGMEnabled;
	}

	public AudioClip paramBGM;

	public void ExecuteChangeBGM()
	{
        Global.Sound.PlayBGM(paramBGM);
	}
}
