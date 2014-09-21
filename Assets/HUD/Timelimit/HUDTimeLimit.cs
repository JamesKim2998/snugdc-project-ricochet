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
		var _mode = Game.Mode as GameModePropertyTimeLimit;

		if (_mode == null) 
		{
			Debug.LogWarning("Mode should support time limit. Ignore.");
			return;
		}
			
		m_TimeLimitLabel.text = _mode.timeLeft.ToString();
	}
}

