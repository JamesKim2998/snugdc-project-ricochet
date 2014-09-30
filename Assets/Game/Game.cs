using UnityEngine;
using System.Collections;

public class Game : Singleton<Game> 
{
	public float timescale = 1f;
	
	public GameCamera camera_ = new GameCamera();
	public static GameCamera Camera_ { get { return Instance.camera_; } }

	public GameAudio audio_ = new GameAudio();
	public static GameAudio Audio { get { return Instance.audio_; } }

	public GameHUD hud;
	public static GameHUD HUD { get { return Instance.hud; } }
    
    public GameBalance balance = new GameBalance();
    public static GameBalance Balance { get { return Instance.balance;  } }

	public GameProgress progress;
	public static GameProgress Progress { get { return Instance.progress; } }
	
	public GameStatistics statistic = new GameStatistics();
	public static GameStatistics Statistic { get { return Instance.statistic; } }

    private GameLevel m_Level;

    public GameLevel level
    {
        get { return m_Level; }
        set
        {
            if (level != null) level.Dispose();
            m_Level = value;
        }
    }

    public static GameLevel Level { get { return Instance.level; } set { Instance.level = value; } }

	public GameModeManager modeManager;
	public static GameModeManager ModeManager { get { return Instance.modeManager; } }
	public static GameMode Mode { get { return ModeManager.mode; } }

	public GameCharacter character;
	public static GameCharacter Character { get { return Instance.character; } } 

	public GameWeapon weapon = new GameWeapon();
	public static GameWeapon Weapon { get { return Instance.weapon; } }

	public GameResult result;
	public static GameResult Result { get { return Instance.result; } }

	public GameCheat cheat;
	public static GameCheat Cheat { get { return Instance.cheat; } }

	private Game() {
	}

	void Awake() 
	{
        ComponentHelper.AssignComponentIfNotExists(gameObject, ref hud);
		ComponentHelper.AssignComponentIfNotExists(gameObject, ref progress);
		ComponentHelper.AssignComponentIfNotExists(gameObject, ref modeManager);
		ComponentHelper.AssignComponentIfNotExists(gameObject, ref result);
        ComponentHelper.AssignComponentIfNotExists(gameObject, ref character);
        ComponentHelper.AssignComponentIfNotExists(gameObject, ref cheat);
    }

	void Start () 
	{
		Time.timeScale = timescale;
		DontDestroyOnLoad(transform.gameObject);

		camera_.Start();
		statistic.Start();

		Global.Server().postDisconnected += ListenBeforeDisconnected;
	}

    private bool m_IsDisposed = false;
    public void Dispose()
    {
        if (m_IsDisposed) return;
        m_IsDisposed = true;
        hud.Dispose();
        result.Dispose();
        statistic.Dispose();
        level = null;

        Global.Server().postDisconnected -= ListenBeforeDisconnected;
    }

    void OnApplicationQuit() { Dispose(); }

	public void Purge()
	{
		character.Purge ();
		hud.Purge ();
		weapon.Purge ();
		level = null;
		modeManager.Purge();
		statistic.Reset();
	}

	void ListenBeforeDisconnected()
	{
		Debug.Log ("Server disconnected.");
		Network.RemoveRPCs(Network.player);
	}
}
