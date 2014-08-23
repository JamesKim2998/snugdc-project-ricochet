using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Scene
{
	MAIN_MENU,
	LOBBY,
	GAME_LEVEL_KKH_001,
	GAME_LEVEL_KDW_001,
}

public static class SceneNames
{
	public static string MAIN_MENU = "ui_mainmenu";
	public static string LOBBY = "ui_room";
	
	public static string[] GAME_LEVEL = {
		"test_map_kkh001",
		"test_map_kdw001",
	};

	private static Dictionary<Scene, string> s_SceneToName;
	public static string GetName(Scene _scene) { return s_SceneToName[_scene]; }

	static SceneNames()
	{
		s_SceneToName = new Dictionary<Scene, string> {
			{ Scene.MAIN_MENU, MAIN_MENU },
			{ Scene.LOBBY, LOBBY },
			{ Scene.GAME_LEVEL_KKH_001, GAME_LEVEL[0] },
			{ Scene.GAME_LEVEL_KDW_001, GAME_LEVEL[1] },
		};
	}
}
