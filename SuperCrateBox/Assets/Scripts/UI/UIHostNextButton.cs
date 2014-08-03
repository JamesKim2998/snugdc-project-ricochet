using UnityEngine;
using System.Collections;

public class UIHostNextButton : MonoBehaviour {

	public PlayMakerFSM fsm;

	public void Fail()
	{
		Debug.Log("Initialize server is failed.");
		fsm.SendEvent ("FAIL");
	}

	public void OnSubmit()
	{
		fsm.SendEvent ("HOST");
		Invoke ("Fail", 0.5f);

		var _hostPort = GlobalVariables.HOST_PORT;
		if (_hostPort != null) Network.InitializeServer(4, _hostPort.Value, false);
		else Debug.LogWarning("Host port does not exist.");
	}

	public void OnServerInitialized()
	{
		CancelInvoke ("Fail");
		fsm.SendEvent ("SUCCESS");
		Global.Context ().context = ContextType.LOBBY;
	}
}
