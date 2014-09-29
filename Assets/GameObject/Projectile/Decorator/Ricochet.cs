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
		var _shouldDecay = true;

		if ((LayerHelper.Exist(penetrationForced, _collider)))
		{
            _shouldDecay = false;
			m_IsPenetrating = true;
			m_IsPenetratingTemp = true;
			m_PenetrationTimer = 0.1f;
			m_PenetratingObject = _collider.gameObject.GetInstanceID();
		    
			if (effectPenetratePrf)
			{
				var _effect = (GameObject) Instantiate(effectPenetratePrf, transform.position, transform.rotation);
				_effect.transform.Translate(effectPenetrateOffset);
			}
		}
		else if ( !m_IsReflected && reflectionCount > 0 
		         && (LayerHelper.Exist(reflectionMask, _collider)))
		{
		    m_IsReflected = Reflect(_collider);
		    if (!m_IsReflected)
		        return false;

		    _shouldDecay = --reflectionCount == 0;

			if (effectReflectPrf)
			{
				var _effect = (GameObject) Instantiate(effectReflectPrf, transform.position, transform.rotation);
				_effect.transform.Translate(effectReflectOffset);
			}
		}

		return _shouldDecay;
	}

	bool Reflect(Collider2D _collider)
	{
        // Debug.Log("reflect");

        var _direction = transform.rigidbody2D.velocity.normalized;

        var _rayResult = Physics2D.Raycast(
            (Vector2) transform.position + _direction * -0.5f,
            _direction, 
			2f, 
			reflectionMask);

        if (_rayResult.collider)
		{
		    if ((_rayResult.point - (Vector2) transform.position).sqrMagnitude < 0.01f)
		        return false;

            var _velocity = transform.rigidbody2D.velocity;
		    var _rotation = Quaternion.FromToRotation(-_direction, _rayResult.normal);
            transform.rigidbody2D.velocity = _rotation * _rotation * -_velocity;
            // Debug.Log(_rayResult.normal);
            return true;
		}
        else
        {
            Debug.Log("RayCast failed!");
            return false;
        }
	}
}
