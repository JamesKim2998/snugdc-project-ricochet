using System.Linq;
using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class ReadyManager : MonoBehaviour 
{
	public bool IsReadyAll()
	{
        return Global.Player().All(_playerKV => (!_playerKV.Value.connected) || _playerKV.Value.isReady || _playerKV.Value == Global.Server().server);
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

	public void Dispose()
	{
		Global.Context ().postChanged -= ListenContextChanged;
	}

	public void ListenContextChanged(ContextType _context, ContextType _old)
	{
	    if (_context == ContextType.GAME)
            Clear();
	}
}
