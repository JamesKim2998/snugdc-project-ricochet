using System.Linq;
using UnityEngine;
using System.Collections;

public class UIReadyButton : MonoBehaviour {
	void Start() 
	{
		Global.Server().postDisconnected += ListenDisconnectedFromServer;
	}

	void OnDestroy()
	{
		Global.Server().postDisconnected -= ListenDisconnectedFromServer;
	}

	void OnServerInitialized()
	{
		gameObject.SetActive(false);
	}

	void ListenDisconnectedFromServer()
	{
		gameObject.SetActive(true);
	}

	public void OnSubmit()
	{
        Global.Player().mine.isReady.val = !Global.Ready().IsReady();
        Global.Player().UpdateInfo();
	}
}
