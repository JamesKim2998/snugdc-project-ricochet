using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameDef : MonoBehaviour
{
	public bool applyOnStart = true;
	public bool applyAfterDelay = false;
	public float applyDelay = -1f;
	public bool deleteAfterApply = true;

	public float timeScale = 1f;

	public GameAudioDef audio_;
	public GameCameraDef camera_;
	public GameHUDDef hud;
	public GameModeManagerDef modeManager;

	public bool useCharacterDef = false;
	public GameCharacterDef character;

	public bool useWeaponDef = false;
	public GameWeaponDef weapon;

	void Start ()
	{
		if (applyOnStart) 
		{
			Apply();
		}
		else if (applyAfterDelay)
		{
			Invoke("Apply", applyDelay);
		}
	}

	void Apply()
	{
		Time.timeScale = timeScale;
		if (audio_ != null) audio_.Apply();
		if (camera_ != null) camera_.Apply();
		if (hud != null) Game.HUD().Apply(hud);
		if (modeManager != null) Game.ModeManager().Apply(modeManager);
		if (useCharacterDef && character != null) character.Apply();
		if (useWeaponDef && weapon != null) weapon.Apply();
		if (deleteAfterApply) Destroy(gameObject);
	}
}

[System.Serializable]
public class GameAudioDef 
{
	public bool pause = false;
	public float volume = 1;
	
	public void Apply()
	{
		AudioListener.pause = pause;
		AudioListener.volume = volume;
	}
}

[System.Serializable]
public class GameCameraDef
{
	public Camera camera;

	public void Apply() 
	{
		if (camera != null) 
		{
			Game.Camera_.camera = camera;
		}
	}
}

[System.Serializable]
public class GameHUDDef 
{
	public GameObject hudRoot;

	public GameObject chatscreenPrf;
	public bool useChatscreenKey = true;
	public KeyCode chatscreenActivateKey = KeyCode.Return;
	public KeyCode chatscreenDeactivateKey = KeyCode.Escape;
	
	public GameObject scoreBoardPrf;
	public bool useScoreBoardActivateKey = true;
	public KeyCode scoreBoardActivateKey = KeyCode.Tab;

	public Vector3 resultBoardPosition = Vector3.zero;
	public GameObject resultBoardPrf;

	[System.Serializable]
	public class ModeHUD {
		public GameModeType mode;
		public GameObject prefab;
	}

	public List<ModeHUD> modeHUDs;
}

[System.Serializable]
public class GameModeManagerDef
{
	public GameMode mode;
}

[System.Serializable]
public class GameCharacterDef
{
	public Character character;

	public bool useDownForce = false;
	public float downForce = 30f;

	public bool useMaxUpForce = false;
	public float maxUpForce = 2f;

	public bool useUpForce = false;
	public float upForce = 30f;
	
	public GameObject weaponDefault;
	
	public List<Color> characterColors;
	
	public void Apply()
	{
		if (useDownForce) Game.Character().downForce = downForce;
		if (useMaxUpForce) Game.Character().maxUpForce = maxUpForce;
		if (useUpForce) Game.Character().upForce = upForce;
		if (characterColors != null && characterColors.Count > 0) 
			Game.Character().characterColors = new List<Color>(characterColors);
		if (weaponDefault) Game.Character().weaponDefault = weaponDefault;
		if (character) Game.Character().character = character;
	}
}

[System.Serializable]
public class GameWeaponDef
{
	public List<string> weaponSet;

	public void Apply()
	{
		if (weaponSet != null && weaponSet.Count > 0) Game.Weapon().weaponSet = new List<string>(weaponSet);
	}
}

