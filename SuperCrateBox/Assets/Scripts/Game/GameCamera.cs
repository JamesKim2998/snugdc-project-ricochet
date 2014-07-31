using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameCamera 
{
	private Camera m_Camera;
	public Camera camera { get { return m_Camera; } set { m_Camera = value; }}

	private CameraFollowObject followObject;

	public void Start() 
	{
		followObject = m_Camera.GetComponent<CameraFollowObject> ();

		if (followObject == null) 
			followObject = m_Camera.gameObject.AddComponent<CameraFollowObject>();
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


