using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class NetworkBridge : MonoBehaviour
{
	public NetworkView global;
	public NetworkView game;

	void Start()
	{
		if (networkView == null
		    || global == null
		    || game == null)
		{
			LogCommon.MissingComponent();
			return;
		}
		
		Global.Instance.networkBridge = this;
		Global.Instance.networkView.viewID = global.viewID;
		global.viewID = NetworkViewID.unassigned;
		Destroy(global);

		Game.Instance.networkView.viewID = game.viewID;
		game.viewID = NetworkViewID.unassigned;
		Destroy(game);
	}

	public void Dispose()
	{
		Global.Instance.networkBridge = null;
	}
}

