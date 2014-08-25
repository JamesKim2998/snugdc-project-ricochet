using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour {
	private static LevelLoader _Instance;
	public static LevelLoader Instance{
		get{
			if(_Instance)
				return _Instance;
			else{
				_Instance = (LevelLoader)FindObjectOfType(typeof(LevelLoader));
				if (_Instance == null){
					GameObject levelLoader = new GameObject();
					_Instance = levelLoader.AddComponent<LevelLoader>();
					levelLoader.name = "(temp)levelLoader";
				}
				return _Instance;
			}
		}
	}

	public virtual void LoadLevel(string _sceneName){
		Application.LoadLevel(_sceneName);
	}
}
