using UnityEngine;
using System.Collections;

public class ExplodeOnCrash : MonoBehaviour {
	
	private Projectile m_Projectile;

	public string explosion = "explosion";

	private bool m_Exploded = false;

	public float duration;
	public float radius;
	
	void Start () 
	{
		m_Projectile = GetComponent<Projectile>();
		m_Projectile.postHit += Explode;
		m_Projectile.postBumped += Explode;
	}

	void Explode(Projectile _projectile, Collider2D _collider) 
	{
		if (m_Exploded) 
		{
			Debug.Log("trying to explode multiple times");
			return;
		}

		RPCExplode(transform.position);

		if (Network.peerType != NetworkPeerType.Disconnected 
		    && networkView.enabled) 
		{
			networkView.RPC("RPCExplode", RPCMode.Others, transform.position);
		}
	}

	[RPC]
	void RPCExplode(Vector3 _position) 
	{
		if (m_Exploded) 
		{
			Debug.Log("trying to explode multiple times");
			return;
		}

		m_Exploded = true;
		
		var _explosionObj = GameObject.Instantiate(Resources.Load(explosion), transform.position, new Quaternion()) as GameObject;
		var _explosion = _explosionObj.GetComponent<Explosion>();
		_explosion.attackData = m_Projectile.attackData;
		_explosion.duration = duration;
		_explosion.radius = radius;
		_explosion.targets = m_Projectile.targets;
		_explosion.Explode();
	}

}
