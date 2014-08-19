using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGameModeTest : MonoBehaviour
{
	public UIPopupList mapSelector;
	private GameModeTestDef m_ModeDef;

	void Start ()
	{
		if (mapSelector == null) 
			mapSelector = GetComponent<UIPopupList>();

		m_ModeDef = Global.GameSetting().modeTest;

		foreach (var _map in SceneNames.GAME_LEVEL)
			mapSelector.items = new List<string>(SceneNames.GAME_LEVEL);

		mapSelector.value = m_ModeDef.testLevel;

		Refresh();
	}

	void Refresh()
	{
		mapSelector.value = m_ModeDef.testLevel;
	}
}

