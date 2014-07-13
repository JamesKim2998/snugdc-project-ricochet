using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameCamera 
{
	private Camera m_Camera;
	public Camera cameraExpose;

	private CameraFollowObject followObject;

	public void Start() 
	{
		if (cameraExpose == null) {
			Debug.LogError("camera not exist!");
			return;
		}

		m_Camera = cameraExpose;

		followObject = m_Camera.GetComponent<CameraFollowObject> ();

		if (followObject == null) 
			Debug.LogError("FollowObject not exist!");
	}

	public void Bind(int _key, Transform _transform)
	{
		// incomplete code
		followObject.target = _transform.gameObject;
	}

	public void Unbind(int _key)
	{
		// incomplete code
		followObject.target = null;
	}
}


