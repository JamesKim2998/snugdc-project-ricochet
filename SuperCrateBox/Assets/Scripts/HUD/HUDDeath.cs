using UnityEngine;
using System.Collections;

public class HUDDeath : MonoBehaviour {
	
	void Start () {
		Game.Statistic().death.postChanged += Set;
	}
	
	void Update () {
		
	}
	
	public void Set(Statistic _statistic) {
		var _text = "Death: " + _statistic.val;
		guiText.text = _text;
		
	}
}
