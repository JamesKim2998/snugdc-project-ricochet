using UnityEngine;
using System.Collections;

public class HUDScore : MonoBehaviour {
	IEnumerator Bind ()
	{
		yield return new WaitForSeconds(0.1f);
		if (Game.Statistic ().myUserStatistic == null) {
			StartCoroutine (Bind());
		} else {
			Game.Statistic().myUserStatistic.score.postChanged += Set;
		}
	}

	void Start () {
		StartCoroutine (Bind());
	}
	
	void Update () {
	
	}

	public void Set(Statistic<int> _statistic) {
		var _text = "Score: " + _statistic.val;
		guiText.text = _text;

	}
}
