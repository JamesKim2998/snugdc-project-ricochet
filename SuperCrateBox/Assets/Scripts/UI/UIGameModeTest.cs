using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UIGameModeTest : MonoBehaviour
{
	private GameModeTestDef m_ModeDef;

	void Start ()
	{
		m_ModeDef = Global.GameSetting().modeTest;
		Refresh();
	}

	void Refresh()
	{
	}
}

