using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CallOnKeyboardEvent : MonoBehaviour {
	public enum Event {
		PRESS,
		RELEASE,
	}

	public KeyCode key;
	public Event event_;

	public List<EventDelegate> onEvent = new List<EventDelegate>();

	void Update () 
	{
		if (event_ == Event.PRESS && Input.GetKeyDown(key))
		{
			EventDelegate.Execute(onEvent);
		}
		else if (event_ == Event.RELEASE && Input.GetKeyUp(key))
		{
			EventDelegate.Execute(onEvent);
		}
	}
}
