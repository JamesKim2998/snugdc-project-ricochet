using UnityEngine;
using System.Collections;

public class UIHostNextButton : MonoBehaviour {

	public UIButton button;
	public string lobbyScene = SceneNames.LOBBY;

	public void Enable()
	{
		Debug.Log("Initialize server is failed.");
		button.isEnabled = true;
	}

	public void OnSubmit()
	{
		button.isEnabled = false;
		Invoke ("Enable", 0.5f);

		var _hostPort = GlobalVariables.HOST_PORT;
		if (_hostPort != null) Network.InitializeServer(4, _hostPort.Value, false);
		else Debug.LogWarning("Host port does not exist.");
	}

	public void OnServerInitialized()
	{
		CancelInvoke ("Enable");
		Application.LoadLevel (lobbyScene);
		Global.Context ().context = ContextType.LOBBY;
	}
}
