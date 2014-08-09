using UnityEngine;
using System.Collections;

public class SendFSMEvent : MonoBehaviour {

	public PlayMakerFSM fsm;
	public string event_;

	public void Send() 
	{
		fsm.SendEvent (event_);
	}
}
