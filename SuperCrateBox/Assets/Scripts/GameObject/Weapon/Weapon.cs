
using UnityEngine;

abstract public class Weapon : MonoBehaviour {

	private GameObject m_Owner;
	public GameObject owner { get { return m_Owner; } set { m_Owner = value; }}
	
	private Vector2 m_Direction = new Vector2(1, 0);
	public Vector2 direction { get { return m_Direction; } set { m_Direction = value.normalized; }}

	public int? damage;

	abstract public bool isShooting { get; }
			 
	abstract public bool isCooling { get; }
	abstract public float cooldown { get; set; }
			 
	abstract public bool IsShootable();
	abstract public void Shoot();
			 
	abstract public void Stop();
}