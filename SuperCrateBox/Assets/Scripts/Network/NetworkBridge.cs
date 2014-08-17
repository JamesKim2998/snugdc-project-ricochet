using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
public class NetworkBridge : MonoBehaviour
{
	public static bool sInitialized = false;

	public NetworkViewAllocator allocator;

	public NetworkView global;
	public NetworkView game;

	void Start()
	{
		if (sInitialized) 
		{
			Destroy(gameObject);
			return;
		}

		if (networkView == null
		    || global == null
		    || game == null)
		{
			Debug.LogError("Missing property!");
		}
		
		sInitialized = true;
		DontDestroyOnLoad(gameObject);

		allocator = gameObject.AddComponent<NetworkViewAllocator>();

		Global.Instance.networkBridge = this;
		Global.Instance.networkView.viewID = global.viewID;
		global.viewID = NetworkViewID.unassigned;

		Game.Instance.networkView.viewID = game.viewID;
		game.viewID = NetworkViewID.unassigned;
	}

	void OnDestroy()
	{
		Global.Instance.networkBridge = null;
	}
}

