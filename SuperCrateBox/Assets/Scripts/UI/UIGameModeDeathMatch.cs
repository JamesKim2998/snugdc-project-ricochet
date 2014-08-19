using UnityEngine;
using System.Collections;

public class UIGameModeDeathMatch : MonoBehaviour {
	public UIInput respawnCountInput;
	public UIInput timeLimitInput;

	private GameModeDeathMatchDef m_GameModeDef;

	void Start () 
	{
		if (respawnCountInput == null
		    || timeLimitInput == null
			|| Global.GameSetting().mode == null)
		{
			Debug.LogError("Missing component!");
			return;
		}

		m_GameModeDef = Global.GameSetting().mode as GameModeDeathMatchDef;
		if (m_GameModeDef == null) 
		{
			Debug.LogError("Mode does not match!");
			return;
		}

		respawnCountInput.onSubmit.Add(new EventDelegate(() =>  m_GameModeDef.respawnCount = int.Parse(respawnCountInput.value)));
		timeLimitInput.onSubmit.Add(new EventDelegate( () => m_GameModeDef.timeLimit = int.Parse(timeLimitInput.value)));
	}
}
