using UnityEngine;
using System.Collections;

public class HUDRespawnCount : MonoBehaviour
{
	public UILabel respawnLimit;
	public UILabel respawnCount;
	
	void Start ()
	{
		if (! (respawnLimit && respawnCount)) {
			LogCommon.MissingComponent();
			enabled = false;
		}
	}
	
	void Update ()
	{
		var _mode = Game.Mode as GameModeDeathMatch;
		
		if (_mode == null) 
		{
//			Debug.LogWarning("Only support death match.");
//			enabled = false;
			return;
		}
		
		respawnLimit.text = _mode.respawnLimit.ToString();
		respawnCount.text = _mode.respawnLeft.ToString();
	}
}

