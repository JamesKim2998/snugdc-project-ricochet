using UnityEngine;
using System.Collections;

public class Agent : MonoBehaviour {

	private float m_Speed = 1;

	private Vector2 m_Direction = new Vector2(0, 0);
	public Vector2 direction { 
		get { return m_Direction; }
		set { m_Direction = value; }
	}

	public virtual float speed { 
		get { return m_Speed; }
		set { m_Speed = value; } 
	}

	public virtual void Start () {
	}
	
	public virtual void Update () {
	
	}
}
