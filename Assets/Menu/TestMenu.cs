using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections;

public static class TestScenes
{
    public const string UI_MAINMENU = "Assets/UI/ui_mainmenu.unity";
    public const string UI_ROOM = "Assets/UI/ui_room.unity";
    public const string MAP_KKH001 = "Assets/Maps/test_map_kkh001.unity";
    public const string MAP_KDW001 = "Assets/Maps/test_map_kdw001.unity";
    public const string GAME_CHARACTER = "Assets/GameObject/Character/Test/test_character.unity";
}

public class TestMenu  {

    private static void TransferScene(string _scene)
    {
        if (Path.GetFileName(EditorApplication.currentScene) == Path.GetFileName(_scene))
            return;
        EditorApplication.SaveCurrentSceneIfUserWantsTo();
        EditorApplication.OpenScene(_scene);   
    }

    [MenuItem("RicochetTest/UI/MainMenu")]
    public static void TestMainMenu()
    {
        TransferScene(TestScenes.UI_MAINMENU);
    }

    [MenuItem("RicochetTest/UI/Room")]
    public static void TestRoom()
    {
        TransferScene(TestScenes.UI_ROOM);
    }

    [MenuItem("RicochetTest/Map/KKH001")]
    public static void TestKKH001()
    {
        TransferScene(TestScenes.MAP_KKH001);
    }

    [MenuItem("RicochetTest/Map/KDW001")]
    public static void TestKDW001()
    {
        TransferScene(TestScenes.MAP_KDW001);
    }

    [MenuItem("RicochetTest/Game/Character")]
    public static void TestCharacter()
    {
        TransferScene(TestScenes.GAME_CHARACTER);
    }
}
