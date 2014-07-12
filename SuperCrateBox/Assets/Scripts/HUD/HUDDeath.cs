using UnityEngine;
using System.Collections;

public class HUDDeath : MonoBehaviour {
	IEnumerator Bind ()
	{
		yield return new WaitForSeconds(0.1f);
		if (Game.Statistic ().myUserStatistic == null) {
			StartCoroutine (Bind());
		} else {
			Game.Statistic().myUserStatistic.death.postChanged += Set;
		}
	}

	void Start () {
		StartCoroutine (Bind());
	}
	
	void Update () {
		
	}
	
	public void Set(Statistic<int> _statistic) {
		var _text = "Death: " + _statistic.val;
		guiText.text = _text;
	}
}
