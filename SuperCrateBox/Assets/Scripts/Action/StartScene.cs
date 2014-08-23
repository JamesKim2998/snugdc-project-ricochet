using UnityEngine;
using System.Collections;

public class StartScene : MonoBehaviour
{
	public Scene scene;

	public void Execute()
	{
		Application.LoadLevel(SceneNames.GetName(scene));
	}
}

