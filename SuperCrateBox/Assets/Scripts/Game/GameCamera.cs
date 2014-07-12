using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameCamera 
{
	private Camera m_Camera;
	public Camera cameraExpose;

	private FollowObject followObject;

	public void Start() 
	{
		if (cameraExpose == null) 
			Debug.LogError("camera not exist!");

		m_Camera = cameraExpose;

		followObject = m_Camera.GetComponent<FollowObject> ();

		if (followObject == null)
			Debug.LogError("FollowObject not exist!");
	}

	public void Pull(string _key, Transform _transform)
	{
		// incomplete code
		followObject.target = _transform.gameObject;
	}

	public void Push(string _key)
	{
		// incomplete code
		followObject.target = null;
	}
}


