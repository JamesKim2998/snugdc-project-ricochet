
using UnityEngine;

abstract public class Weapon : MonoBehaviour 
{
	public string type;

	private GameObject m_Owner;
	public GameObject owner { get { return m_Owner; } set { m_Owner = value; }}

	public int? damage;

	abstract public bool isShooting { get; }
			 
	abstract public bool isCooling { get; }
	abstract public float cooldown { get; set; }
			 
	abstract public bool IsShootable();
	abstract public void Shoot();
			 
	abstract public void Stop();
}