using UnityEngine;
using System.Collections;

public class UIJoinNextButton : MonoBehaviour {
	public PlayMakerFSM fsm;

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

	public void Fail()
	{
		Debug.Log("Join to server is failed.");
		fsm.SendEvent ("FAIL");
	}

	public void OnConnectedToServer()
	{
		fsm.SendEvent ("SUCCESS");
		Global.Context ().context = ContextType.LOBBY;
	}
}
