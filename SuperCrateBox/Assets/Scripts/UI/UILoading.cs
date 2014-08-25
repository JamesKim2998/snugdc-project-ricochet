using UnityEngine;
using System.Collections;

public class UILoading : LevelLoader {
	public float imagePosition = 16f;
	string sceneName;
	void Start(){
		iTween.MoveTo(gameObject,iTween.Hash("position",new Vector3(0,imagePosition,10),
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
		iTween.MoveTo(gameObject,iTween.Hash("position",new Vector3(0,0,10),
		                                     "time",0.5f,
		                                     "oncomplete","loadScene",
		                                     "oncompletetarget",gameObject,
		                                     "islocal",true));
	}
	void loadScene(){
		Application.LoadLevel(sceneName);
	}
}
