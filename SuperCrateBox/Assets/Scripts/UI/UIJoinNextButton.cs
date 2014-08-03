using UnityEngine;
using System.Collections;

public class UIJoinNextButton : MonoBehaviour {
	public PlayMakerFSM fsm;
	
	public void Fail()
	{
		Debug.Log("Join to server is failed.");
		fsm.SendEvent ("FAIL");
	}
	
	public void OnSubmit()
	{
		fsm.SendEvent ("JOIN");
		Invoke ("Fail", 0.5f);

		if (GlobalVariables.JOIN_IP == null 
		    || GlobalVariables.JOIN_PORT == null )
		{
			Debug.LogWarning("Join IP or port do not exist.");
			return;
		}
		
		Network.Connect(GlobalVariables.JOIN_IP, GlobalVariables.JOIN_PORT.Value);
	}

	bool m_Connected = false;

	// todo: 버그로 인해서 다시 접속을 요청합니다.
	public void OnConnectedToServer()
	{
		if (m_Connected) 
		{
			ConnectAgain();
			return;
		}

		m_Connected = true;
		Network.Disconnect ();
		Network.Connect(GlobalVariables.JOIN_IP, GlobalVariables.JOIN_PORT.Value);
	}

	void ConnectAgain() 
	{
		fsm.SendEvent ("SUCCESS");
		Global.Context ().context = ContextType.LOBBY;
	}
}
