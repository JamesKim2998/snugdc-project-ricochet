using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneInfo
{
	public SceneInfo(Scene _scene, string _name) 
	{
		scene = _scene;
		name = _name;
	}

	public Scene scene;
	public string name;
}

public static class SceneInfos
{
	private static Dictionary<Scene, SceneInfo> s_SceneInfos;
	public static SceneInfo Get(Scene _scene) { return s_SceneInfos[_scene]; }

	public static List<SceneInfo> gameMaps;
	public static SceneInfo GameMap(int idx) 
	{ 
		if (idx >= gameMaps.Count) {
			Debug.LogWarning("Not existing level " + idx + ".");
			return null;
		}
		return gameMaps[idx];
	}


	static SceneInfos()
	{
		gameMaps = new List<SceneInfo> {
			new SceneInfo(Scene.GAME_LEVEL_KKH_001, SceneNames.GAME_LEVEL_KKH_001),
			new SceneInfo(Scene.GAME_LEVEL_KDW_001, SceneNames.GAME_LEVEL_KDW_001),
		};

		s_SceneInfos = new Dictionary<Scene, SceneInfo> {
			{ Scene.MAIN_MENU, new SceneInfo(Scene.MAIN_MENU, SceneNames.MAIN_MENU) },
			{ Scene.LOBBY, new SceneInfo(Scene.LOBBY, SceneNames.LOBBY) },
		};

		foreach (var _sceneInfo in gameMaps) {
			s_SceneInfos.Add(_sceneInfo.scene, _sceneInfo);
		}
	}
}
