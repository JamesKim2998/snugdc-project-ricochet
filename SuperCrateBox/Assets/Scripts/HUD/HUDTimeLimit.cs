using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UILabel))]
public class HUDTimeLimit : MonoBehaviour
{
	private UILabel m_TimeLimitLabel;

	void Start ()
	{
		m_TimeLimitLabel = GetComponent<UILabel>();
	}

	void Update ()
	{
		var _mode = Game.Mode() as GameModeDeathMatch;

		if (_mode == null) 
		{
//			Debug.LogWarning("Only support death match.");
//			enabled = false;
			return;
		}
			
		m_TimeLimitLabel.text = _mode.timeLeft.ToString();
	}
}

