using UnityEngine;
using System;
using System.Collections;

public class ServerManager : MonoBehaviour
{
	public Action postServerInitialized;
	public Action postConnected;
	public Action postDisconnected;

	void OnServerInitialized()
	{
		if (postServerInitialized != null) postServerInitialized();
	}

	void OnConnectedToServer()
	{
		if (postConnected != null) postConnected();
	}

	void OnDisconnectedFromServer()
	{
		if (postDisconnected != null) postDisconnected();
	}
}

