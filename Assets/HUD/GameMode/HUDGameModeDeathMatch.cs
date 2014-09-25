using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class HUDGameModeDeathMatch : MonoBehaviour
{
    public List<GameObject> hudNonAnchoredPrfs; 
    public List<HUDAnchor> hudPrfs;

    private readonly List<GameObject> m_HUDNonAnchoreds = new List<GameObject>();
    private readonly List<GameObject> m_HUDs = new List<GameObject>();

    private bool m_IsSetuped = false;

    void Start()
    {
        TrySetup();
    }

    bool TrySetup()
    {
        if (m_IsSetuped) 
            return true;

        if (Game.Mode)
        {
            Setup();
            return true;
        }
        else
        {
            Invoke("TrySetup", 0.5f);
            return false;
        }
    }

    void Setup()
    {
        m_IsSetuped = true;

        var _mode = Game.Mode;

        if (! _mode)
        {
            Debug.LogError("Mode does not exist. Ignore.");
            return;
        }

        if (_mode.type != GameModeType.DEATH_MATCH)
        {
            Debug.LogWarning("Mode should be " + GameModeType.DEATH_MATCH 
                + ", but is now " + Game.Mode.type + " not match. Continue anyway.");
        }

        foreach (var _hudPrf in hudNonAnchoredPrfs)
        {
            var _hud = (GameObject)Instantiate(_hudPrf.gameObject);
            _hud.transform.parent = transform;
            _hud.transform.localPosition = Vector3.zero;
            _hud.transform.localScale = Vector3.one;
            m_HUDNonAnchoreds.Add(_hud);
        }

	    foreach (var _hudPrf in hudPrfs)
	    {
	        var _hud = (GameObject) Instantiate(_hudPrf.gameObject);
	        _hud.transform.parent = transform;
	        _hud.transform.localPosition = Vector3.zero;
            _hud.transform.localScale = Vector3.one;
            m_HUDs.Add(_hud);
	    }
	}

}
