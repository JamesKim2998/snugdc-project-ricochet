using UnityEngine;
using System.Collections;

public class UILoading : LevelLoader {
	public bool isMainMenu = false;
	string sceneName;
	void Start(){
		if(!isMainMenu)
			iTween.MoveTo(gameObject,iTween.Hash("position",Vector3.up*16f,
			                                     "time",1f,
			                                     "oncomplete","startScene",
			                                     "oncompletetarget",gameObject,
			                                       "easetype",iTween.EaseType.easeInCirc,
			                                       "delay",0.5f,
			                                       "islocal",true));
	}
	void startScene(){
//		gameObject.SetActive(false);
	}
	override public void LoadLevel(string _sceneName){
		sceneName = _sceneName;
		iTween.MoveTo(gameObject,iTween.Hash("position",Vector3.zero,
		                                     "time",0.5f,
		                                     "oncomplete","loadScene",
		                                     "oncompletetarget",gameObject,
		                                     "islocal",true));
	}
	void loadScene(){
		Application.LoadLevel(sceneName);
	}
}
