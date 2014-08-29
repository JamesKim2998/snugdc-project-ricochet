using UnityEngine;
using System.Collections;

public class AngleConstraint : MonoBehaviour {

	[SerializeField]
	private int m_From = 0;

	public int from { 
		get { return m_From; } 
		set { 
			m_From = value; 
			ApplyConstraint(); 
		}
	}

	[SerializeField]
	private int m_Range = 360;

	public int range { 
		get { return m_Range; } 
		set { 
			m_Range = value; 
			ApplyConstraint(); 
		}
	}

	void Update()
	{
		ApplyConstraint();
	}

	void ApplyConstraint()
	{
		var _point = transform.localEulerAngles.z;

		var _margin = (180 - range / 2);
		var _begin = from - _margin;

		var _repeat = (_point - _begin) / 360f;
		var _repeatInt = Mathf.FloorToInt(_repeat);
		var _repeatFract = _repeat - _repeatInt;

		var _pointOffseted = _repeatFract * 360;
		_pointOffseted = Mathf.Clamp(_pointOffseted, _margin, _margin + range);
		_point = _pointOffseted + _begin + _repeatInt * 360;

		var _angle = transform.localEulerAngles;
		_angle.z = _point;
		transform.localEulerAngles = _angle;
	}
}
