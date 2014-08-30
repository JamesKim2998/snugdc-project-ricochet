using UnityEngine;
using System.Collections;

[System.Serializable]
public class GameCamera 
{
	private Camera m_Camera;
	public Camera camera { 
		get { return m_Camera; } 
		set { 
			m_Camera = value; 

			if (m_Camera == null) return;

			followObject = m_Camera.GetComponent<CameraFollowObject> ();
			
			if (followObject == null) 
				followObject = m_Camera.gameObject.AddComponent<CameraFollowObject>();
		}
	}

	private CameraFollowObject followObject;

	public void Start() 
	{}

	public void Bind(int _key, Transform _transform)
	{
		if (! followObject) return;
		followObject.target = _transform.gameObject;
	}

	public void Unbind(int _key)
	{
		if (! followObject) return;
		followObject.target = null;
	}
}


