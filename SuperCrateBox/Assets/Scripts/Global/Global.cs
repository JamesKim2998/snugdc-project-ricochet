﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NetworkView))]
[RequireComponent(typeof(ServerManager))]
[RequireComponent(typeof(PlayerManager))]
[RequireComponent(typeof(ReadyManager))]
[RequireComponent(typeof(TransitionManager))]
public class Global : Singleton<Global> 
{
	[HideInInspector]
	public ServerManager server;
	public static ServerManager Server() { return Instance.server; }

	[HideInInspector]
	public PlayerManager player;
	public static PlayerManager Player() { return Instance.player; }

	[HideInInspector]
	public ReadyManager ready;
	public static ReadyManager Ready() { return Instance.ready; }

	[HideInInspector]
	public TransitionManager transition;
	public static TransitionManager Transition() { return Instance.transition; }

	void Awake () {
		server = GetComponent<ServerManager>();
		if (server == null) server = gameObject.AddComponent<ServerManager>();

		player = GetComponent<PlayerManager>();
		if (player == null) player = gameObject.AddComponent<PlayerManager>();

		ready = GetComponent<ReadyManager>();
		if (ready == null) ready = gameObject.AddComponent<ReadyManager>();

		transition = GetComponent<TransitionManager>();
		if (transition == null) transition = gameObject.AddComponent<TransitionManager>();

		if (networkView == null) gameObject.AddComponent<NetworkView>();
		networkView.stateSynchronization = NetworkStateSynchronization.Off;
		networkView.observed = null;
	}

	void Start () {
		DontDestroyOnLoad(transform.gameObject);
	}

}
