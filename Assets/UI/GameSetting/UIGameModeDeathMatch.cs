using UnityEngine;
using System.Collections;

public class UIGameModeDeathMatch : MonoBehaviour {
	public UIInput respawnCountInput;
	public UIInput timeLimitInput;

	private GameModeDeathMatchDef m_ModeDef;

	void Start () 
	{
		if (respawnCountInput == null
		    || timeLimitInput == null
			|| Global.GameSetting().modeSelected == null)
		{
			Debug.LogError("Missing component!");
			return;
		}

		m_ModeDef = Global.GameSetting().modeDeathMatch;
		if (m_ModeDef == null) 
		{
			Debug.LogError("Mode does not match!");
			return;
		}

		respawnCountInput.onSubmit.Add(new EventDelegate(() =>  m_ModeDef.respawnCount = int.Parse(respawnCountInput.value)));
		timeLimitInput.onSubmit.Add(new EventDelegate( () => m_ModeDef.timeLimit = int.Parse(timeLimitInput.value)));

		Refresh();
	}

	void Refresh()
	{
		respawnCountInput.value = m_ModeDef.respawnCount.ToString();
		timeLimitInput.value = m_ModeDef.timeLimit.ToString();
	}
}