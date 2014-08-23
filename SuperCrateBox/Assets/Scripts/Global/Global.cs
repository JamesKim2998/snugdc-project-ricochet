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

	[HideInInspector]
	public ContextManager context;
	public static ContextManager Context() { return Instance.context; }

	[HideInInspector]
	public System.Random random;
	public static System.Random Random() { return Instance.random; }

	[HideInInspector]
	public AudioSource bgm;
	public static AudioSource BGM() { return Instance.bgm; }
	
	[HideInInspector]
	public AudioSource sfx;
	public static AudioSource SFX() { return Instance.sfx; }

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
	public GameSetting gameSetting;
	public static GameSetting GameSetting() { return Instance.gameSetting; }

	void Awake () {
		context = new ContextManager ();
		random = new System.Random ();

		bgm = gameObject.AddComponent<AudioSource>();
		sfx = gameObject.AddComponent<AudioSource>();

		if (networkView == null) gameObject.AddComponent<NetworkView>();
		networkView.stateSynchronization = NetworkStateSynchronization.Off;
		networkView.observed = null;

		server = GetComponent<ServerManager>();
		if (server == null) server = gameObject.AddComponent<ServerManager>();

		player = GetComponent<PlayerManager>();
		if (player == null) player = gameObject.AddComponent<PlayerManager>();

		ready = GetComponent<ReadyManager>();
		if (ready == null) ready = gameObject.AddComponent<ReadyManager>();

		transition = GetComponent<TransitionManager>();
		if (transition == null) transition = gameObject.AddComponent<TransitionManager>();
	}

	void Start () 
	{
		ready.Start ();
	}

	bool m_IsLoaded = false;

	void load()
	{
		m_IsLoaded = true;

	}

	void save()
	{
		if (! m_IsLoaded)
		{
			Debug.LogWarning("Trying to save but not loaded yet! Ignore.");
			return;
		}
	}

	void OnApplicationQuit()
	{
		save ();
	}
}
