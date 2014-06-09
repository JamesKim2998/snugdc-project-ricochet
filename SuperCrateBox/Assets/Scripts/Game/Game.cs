using UnityEngine;
using System.Collections;

public class Game : Singleton<Game> {

	private Game() {}

	public GamePlayer player = new GamePlayer();
	public static GamePlayer Player() { return Instance.player; } 

	public GameStatistics statistic = new GameStatistics();
	public static GameStatistics Statistic() { return Instance.statistic; }
	
	public GameWeapon weapon = new GameWeapon();
	public static GameWeapon Weapon() { return Instance.weapon; }

	void Start () {

	}
	
	void Update () {
		player.Update();
	}

	void FixedUpdate() {
		player.FixedUpdate();
	}

}

