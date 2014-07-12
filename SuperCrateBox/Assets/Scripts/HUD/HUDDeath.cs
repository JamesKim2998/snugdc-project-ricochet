using UnityEngine;
using System.Collections;

public class HUDDeath : MonoBehaviour {
	
	void Start () {
		Game.Statistic().myUserStatistic.death.postChanged += Set;
	}
	
	void Update () {
		
	}
	
	public void Set(Statistic<int> _statistic) {
		var _text = "Death: " + _statistic.val;
		guiText.text = _text;
		
	}
}
