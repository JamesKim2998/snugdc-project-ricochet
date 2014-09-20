using UnityEngine;
using System.Collections;

public class UIJoinNextButton : MonoBehaviour {
	public PlayMakerFSM fsm;
	public AudioClip successSound;
	public AudioClip failSound;

	public string joinEvent = "JOIN";
	public string successEvent = "SUCCESS";
	public string failEvent = "FAIL";

	public void OnSubmit()
	{
		fsm.SendEvent (joinEvent);
		Invoke ("Fail", 0.5f);

		if (GlobalVariables.JOIN_IP == null 
		    || GlobalVariables.JOIN_PORT == null )
		{
			Debug.LogWarning("Join IP or port do not exist.");
			return;
		}
		
		Global.Server().Connect(GlobalVariables.JOIN_IP, GlobalVariables.JOIN_PORT.Value);
	}

	public void Success()
	{
		CancelInvoke ("Fail");

		if (successSound != null)
			Global.SFX.PlayOneShot(successSound);

		fsm.SendEvent (successEvent);
			
		Global.Context ().context = ContextType.LOBBY;
	}

	public void Fail()
	{
		Debug.Log("Join to server is failed.");
		fsm.SendEvent (failEvent);

		if (failSound != null)
			Global.SFX.PlayOneShot(failSound);
	}

	public void OnConnectedToServer()
	{
		Success();
	}

	void OnFailedToConnect(NetworkConnectionError error) 
	{
		CancelInvoke ("Fail");
		Fail();
	}
}
