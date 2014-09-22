using System.Linq;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class ReadyManager : MonoBehaviour 
{
    public bool IsReady() { return Global.Player().mine.isReady; }

	public bool IsReady(string _player) { 
		if (_player ==  Global.Player().server)
		{
		    Global.Player().server.isReady.val = true;
			return true;
		}

	    var _playerInfo = Global.Player()[_player];
	    return (_playerInfo != null) ? _playerInfo.isReady : false;
	}

	public bool IsReadyAll()
	{
	    return Global.Player().All(_playerKV => IsReady(_playerKV.Key));
	}

    public void Clear()
    {
        foreach (var _playerInfo in Global.Player())
            _playerInfo.Value.isReady.val = _playerInfo.Key == Global.Server().server;
    }

    void Start()
	{
		Global.Context ().postChanged += ListenContextChanged;
	}

	void OnDestroy()
	{
		Global.Context ().postChanged -= ListenContextChanged;
	}

	public void ListenContextChanged(ContextType _context, ContextType _old)
	{
	    if (_context == ContextType.GAME)
            Clear();
	}
}
