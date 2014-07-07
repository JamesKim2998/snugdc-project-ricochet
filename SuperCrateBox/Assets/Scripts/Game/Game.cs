using UnityEngine;
using System.Collections;

public class Game : Singleton<Game> 
{
	private Game() {}

	public float timescale = 1f;

	public GameCharacter character = new GameCharacter();
	public static GameCharacter Character() { return Instance.character; } 

	public GameStatistics statistic = new GameStatistics();
	public static GameStatistics Statistic() { return Instance.statistic; }
	
	public GameWeapon weapon = new GameWeapon();
	public static GameWeapon Weapon() { return Instance.weapon; }

	private static GameCheat m_Cheat = null;
	public static GameCheat Cheat() 
	{ 
		if (m_Cheat == null) 
			m_Cheat = Instance.GetComponent<GameCheat>(); 
		return m_Cheat;
	}

	void Start () {
		Time.timeScale = timescale;
		DontDestroyOnLoad(transform.gameObject);
	}
	
	void Update () {
		character.Update();
	}

	void FixedUpdate() {
		character.FixedUpdate();
	}

}

