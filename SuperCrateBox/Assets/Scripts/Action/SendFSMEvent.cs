using UnityEngine;
using System.Collections;

public class SendFSMEvent : MonoBehaviour {

	public PlayMakerFSM fsm;
	public string event_;

	public void Send() 
	{
		if (fsm)
		{
			fsm.SendEvent (event_);
		}
		else
		{
			Debug.Log("FSM is not specified.");
		}
	}
}
