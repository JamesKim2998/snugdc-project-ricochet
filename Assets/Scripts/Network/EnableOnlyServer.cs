using UnityEngine;
using System.Collections;

public class EnableOnlyServer : MonoBehaviour {

	void Start () 
	{
		gameObject.SetActive(Network.peerType == NetworkPeerType.Server);
		Global.Server().postServerInitialized += ListenServerInitialized;
		Global.Server().postDisconnected += ListenDisconnected;
	}

	void OnDestroy() 
	{
		Global.Server().postServerInitialized -= ListenServerInitialized;
		Global.Server().postDisconnected -= ListenDisconnected;
	}

	void ListenServerInitialized()
	{
		gameObject.SetActive(true);
	}

	void ListenDisconnected()
	{
		gameObject.SetActive(false);
	}
}
