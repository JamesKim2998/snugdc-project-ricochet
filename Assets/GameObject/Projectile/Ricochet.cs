using System.Linq;
using UnityEngine;
using System.Collections;

public class Ricochet : MonoBehaviour 
{
	// penetration
	public int penetration;
	public LayerMask penetrationForced;
	private bool m_IsPenetrating = false;
	private bool m_IsPenetratingTemp = false;
	private float m_PenetrationTimer = 0;
	private int m_PenetratingObject = 0;

	// reflection
	public int reflectionCount;
	private bool m_IsReflected = false;
	public LayerMask reflectionMask;

	// effect
	public GameObject effectReflectPrf;
	public Vector3 effectReflectOffset;

	public GameObject effectPenetratePrf;
	public Vector3 effectPenetrateOffset;

	void Start () 
	{
	}
	
	void Update () 
	{
		m_IsPenetrating = m_IsPenetratingTemp;
		m_IsPenetratingTemp = false;
	}

	void FixedUpdate()
	{
		m_IsReflected = false;

		if (m_PenetrationTimer >= 0)
			m_PenetrationTimer -= Time.fixedDeltaTime;
	}

	public bool ShouldCollide(Collider2D _collider)
	{
		if (m_IsPenetrating || (m_PenetratingObject == _collider.gameObject.GetInstanceID()))
			return false;
		
		if ((m_PenetrationTimer > 0) || m_IsReflected) 
			return false;

		return true;
	}

	public bool OnCollision(Collider2D _collider) 
	{
		bool _shouldDecay = false;

		if ((LayerHelper.Exist(penetrationForced, _collider)
		     //				|| (penetration > _collider.hardness)
		     ))
		{
			m_IsPenetrating = true;
			m_IsPenetratingTemp = true;
			m_PenetrationTimer = 0.1f;
			m_PenetratingObject = _collider.gameObject.GetInstanceID();
			_shouldDecay = true;

			// todo: server
			if (effectPenetratePrf)
			{
				var _effect = (GameObject) Instantiate(effectPenetratePrf, transform.position, transform.rotation);
				_effect.transform.Translate(effectPenetrateOffset);
			}
		}
		else if ( reflectionCount > 0 
		         && (LayerHelper.Exist(reflectionMask, _collider)
		    //		    	|| _collider.reflectable)
		    ))
		{
			--reflectionCount;
			Reflect(_collider);
			_shouldDecay = true;
			
			// todo: server
			if (effectReflectPrf)
			{
				var _effect = (GameObject) Instantiate(effectReflectPrf, transform.position, transform.rotation);
				_effect.transform.Translate(effectReflectOffset);
			}
		}

		return _shouldDecay;
	}

	void Reflect(Collider2D _collider)
	{
	    if (m_IsReflected)
	        return;

		m_IsReflected = true;

        var _direction = transform.TransformDirection(Vector3.right);

        var _rayResult = Physics2D.Raycast(
            transform.TransformPoint(0.5f * Vector3.left),
            _direction, 
			1f, 
			reflectionMask);

        if (_rayResult.collider)
		{
		    var _velocity = transform.rigidbody2D.velocity;
		    transform.rigidbody2D.velocity = Vector3.Reflect(_velocity, _rayResult.normal);
		}
        else
        {
            Debug.Log("RayCast failed!");
        }

	}
}
