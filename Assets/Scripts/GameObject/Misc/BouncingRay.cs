using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BouncingRay : MonoBehaviour {

	public float length = 100f;
	public int bounce = 100;
	public LayerMask mask;

	public GameObject rayPrf;
	public float rayScale = 1;
	public Color rayColor = Color.white;

	private List<GameObject> m_Rays;
	private float m_RayWidth;

	void Start()
	{
		m_Rays = new List<GameObject>();
		m_RayWidth = rayPrf.GetComponent<SpriteRenderer>().sprite.rect.width / 100;
//		Debug.Log(m_RayWidth);
	}

	void Update () 
	{
//		if (! transform.hasChanged)
//			return;

		var _i = 0;
		var _lengthLeft = length;

		Vector2 _rayOrigin = transform.position;
		Vector2 _rayDirection = transform.TransformDirection(Vector3.right);

		while (_lengthLeft > 0 && _i <= bounce)
		{
			GameObject _ray;

			if (m_Rays.Count > _i)
			{
				_ray = m_Rays[_i];
			} 
			else 
			{
				_ray = GameObject.Instantiate(rayPrf) as GameObject;
				_ray.transform.parent = transform;
				m_Rays.Add(_ray);
			}

			_ray.transform.position = _rayOrigin;
			_ray.transform.eulerAngles = new Vector3(0, 0, TransformHelper.VectorToDeg(_rayDirection));
			_ray.GetComponent<SpriteRenderer>().color = rayColor;
			var _scale = _ray.transform.localScale;
			_scale.y = rayScale;

			var _hit = Physics2D.Raycast(_rayOrigin + _rayDirection * 0.1f, _rayDirection, _lengthLeft, mask);

			if  (_hit)
			{
				var _length = Vector2.Distance(_rayOrigin, _hit.point);
				_lengthLeft -= _length;

				_scale.x = _length / m_RayWidth;
				_ray.transform.localScale = _scale;

				_rayOrigin = _hit.point;
				_rayDirection = Vector3.Reflect(_rayDirection, _hit.normal);
			}
			else 
			{
				_scale.x = _lengthLeft / m_RayWidth;
				_ray.transform.localScale = _scale;
				++_i;
				break;
			}

			++_i;
		}

		int _removeBegin = _i;
		for (; _i < m_Rays.Count; ++_i)
			GameObject.Destroy(m_Rays[_i]);
		m_Rays.RemoveRange(_removeBegin, m_Rays.Count - _removeBegin);
	}
}
