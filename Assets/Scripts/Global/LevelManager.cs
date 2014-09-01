using UnityEngine;
using System.Collections;

public class LevelManager {
	public void Load(Scene _scene)
	{
		Application.LoadLevel(SceneInfos.Get(_scene).name);
	}
}
