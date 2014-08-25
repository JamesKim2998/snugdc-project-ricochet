using UnityEngine;
using System.Collections;

public class LevelLoader : Singleton<LevelLoader> {
	public virtual void LoadLevel(string _sceneName){
		Application.LoadLevel(_sceneName);
	}
}
