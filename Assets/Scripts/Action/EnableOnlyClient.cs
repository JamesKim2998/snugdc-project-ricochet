using UnityEngine;
using System.Collections;

public class EnableOnlyClient : MonoBehaviour {
	
	void Start () 
	{
		gameObject.SetActive(Network.peerType == NetworkPeerType.Client);
		Global.Server().postServerInitialized += ListenServerInitialized;
		Global.Server().postConnectionSetuped += ListenConnectionSetuped;
		Global.Server().postDisconnected += ListenDisconnected;
	}
	
	void OnDestroy() 
	{
		Global.Server().postServerInitialized -= ListenServerInitialized;
		Global.Server().postConnectionSetuped -= ListenConnectionSetuped;
		Global.Server().postDisconnected -= ListenDisconnected;
	}
	
	void ListenServerInitialized()
	{
		gameObject.SetActive(false);
	}

	void ListenConnectionSetuped()
	{
		gameObject.SetActive(true);
	}

	void ListenDisconnected()
	{
		gameObject.SetActive(false);
	}
}
