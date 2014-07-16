using UnityEngine;
using System.Collections;

public class Game : Singleton<Game> 
{
	private Game() {}

	public float timescale = 1f;
	public bool multiplayer = false;
	
	public GameCamera camera_ = new GameCamera();
	public static GameCamera Camera() { return Instance.camera_; }

	public GameAudio audio_ = new GameAudio();
	public static GameAudio Audio() { return Instance.audio_; }

	public GameCharacter character = new GameCharacter();
	public static GameCharacter Character() { return Instance.character; } 

	public GameStatistics statistic = new GameStatistics();
	public static GameStatistics Statistic() { return Instance.statistic; }
	
	public GameWeapon weapon = new GameWeapon();
	public static GameWeapon Weapon() { return Instance.weapon; }

	public static GameCheat cheat = null;
	public static GameCheat Cheat() 
	{ 
		if (cheat == null) 
			cheat = Instance.GetComponent<GameCheat>(); 
		return cheat;
	}

	void Start () {
		Time.timeScale = timescale;
		DontDestroyOnLoad(transform.gameObject);

		camera_.Start();
		audio_.Start();

		statistic.Start();

		character.game = this;
		character.Start();
	}
	
	void Update () {
		character.Update();
	}

	void FixedUpdate() {
		character.FixedUpdate();
	}

}

