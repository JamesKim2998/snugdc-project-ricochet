using UnityEngine;
using System.Collections;

public class UIJoinNextButton : MonoBehaviour {
	public UIButton button;
	public string lobbyScene = SceneNames.LOBBY;
	
	public void Enable()
	{
		Debug.Log("Join to server is failed.");
		button.isEnabled = true;
	}
	
	public void OnSubmit()
	{
		button.isEnabled = false;
		Invoke ("Enable", 0.5f);

		if (GlobalVariables.JOIN_IP == null 
		    || GlobalVariables.JOIN_PORT == null )
		{
			Debug.LogWarning("Join IP or port do not exist.");
			return;
		}
		
		Network.Connect(GlobalVariables.JOIN_IP, GlobalVariables.JOIN_PORT.Value);
	}

	public void OnConnectedToServer()
	{
		CancelInvoke ("Enable");
		Application.LoadLevel (lobbyScene);
		Global.Context ().context = ContextType.LOBBY;
	}
}
