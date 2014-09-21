using UnityEngine;
using System.Collections;

public class HUDRespawnCount : MonoBehaviour
{
	public UILabel respawnLimit;
	public UILabel respawnCount;
	
	void Start ()
    {
        if (!(respawnLimit && respawnCount))
        {
            LogCommon.MissingComponent();
            enabled = false;
        }
	}
	
	void Update ()
	{
		var _mode = Game.Mode as GameModePropertyRespawn;
		
		if (_mode == null) 
		{
            Debug.LogWarning("Mode should support respawn. Ignore.");
			return;
		}
		
		respawnLimit.text = _mode.respawnLimit.ToString();
		respawnCount.text = _mode.respawnLeft.ToString();
	}
}

