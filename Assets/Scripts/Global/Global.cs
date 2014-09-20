using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
[RequireComponent(typeof(ServerManager))]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(ReadyManager))]
[RequireComponent(typeof(TransitionManager))]
public class Global : Singleton<Global> 
{
	public static new Global Instance { get { return Singleton<Global>.Instance; } }

	public ContextManager context = new ContextManager ();
	public static ContextManager Context() { return Instance.context; }

    public System.Random random = new System.Random();
	public static System.Random Random() { return Instance.random; }

    public OptionManager option;
    public static OptionManager Option { get { return Instance.option; } }

    public ConfigurationManager config = new ConfigurationManager();
    public static ConfigurationManager Config { get { return Instance.config;  } }

	public LevelManager level = new LevelManager();
	public static LevelManager Level { get { return Instance.level; } }

	public static AudioSource BGM { get { return Sound.bgm; } }
	public static AudioSource SFX { get { return Sound.sfx; } }

    public SoundManager sound = new SoundManager();
    public static SoundManager Sound { get { return Instance.sound; } }

	[HideInInspector]
	public ServerManager server;
	public static ServerManager Server() { return Instance.server; }
	
	[HideInInspector]
	public NetworkBridge networkBridge;
	public static NetworkBridge NetworkBrigde() { return Instance.networkBridge; }

	[HideInInspector]
	public PlayerManager player;
	public static PlayerManager Player() { return Instance.player; }

	[HideInInspector]
	public ReadyManager ready;
	public static ReadyManager Ready() { return Instance.ready; }

	[HideInInspector]
	public TransitionManager transition;
	public static TransitionManager Transition() { return Instance.transition; }

	[HideInInspector]
	public CacheManager localCache = new CacheManager();
	public static CacheManager LocalCache() { return Instance.localCache; }

	// please implement this.
	/*
	[HideInInspector]
	public CacheManager networkCache = new 
	*/
	
	[HideInInspector]
	public GameSetting gameSetting = new GameSetting();
	public static GameSetting GameSetting() { return Instance.gameSetting; }

	void Awake () {
        config.Load();
	    
		if (networkView == null) gameObject.AddComponent<NetworkView>();
		networkView.stateSynchronization = NetworkStateSynchronization.Off;
		networkView.observed = null;

        option = ComponentHelper.AddComponentIfNotExists<OptionManager>(gameObject);
        sound = ComponentHelper.AddComponentIfNotExists<SoundManager>(gameObject);
        server = ComponentHelper.AddComponentIfNotExists<ServerManager>(gameObject);
        player = ComponentHelper.AddComponentIfNotExists<PlayerManager>(gameObject);
        ready = ComponentHelper.AddComponentIfNotExists<ReadyManager>(gameObject);
        transition = ComponentHelper.AddComponentIfNotExists<TransitionManager>(gameObject);
	}

	void Start () 
	{
		ready.Start ();
	}

	bool m_IsLoaded = false;

	void Load()
	{
		m_IsLoaded = true;

	}

	void Save()
	{
		if (! m_IsLoaded)
		{
			Debug.LogWarning("Trying to save but not loaded yet! Ignore.");
			return;
		}
	}

	void OnApplicationQuit()
	{
		Save ();
	}
}
