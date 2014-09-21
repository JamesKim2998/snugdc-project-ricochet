using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class HUDGameModeDeathMatch : MonoBehaviour
{
    public List<HUDAnchor> hudPrfs;
    private readonly List<GameObject> m_HUDs = new List<GameObject>();

	void Start () {
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
