using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.Collections;

public class HUDGameModeDeathMatch : MonoBehaviour
{
    public List<HUDAnchor> hudPrfs;
    private readonly List<GameObject> m_HUDs = new List<GameObject>();

    void Start()
    {
        var _mode = Game.Mode as GameModeDeathMatch;
        if (_mode)
        {
            Debug.LogWarning("Mode should be " + GameModeType.DEATH_MATCH 
                + ", but is now " + Game.Mode.type + " not match. Continue anyway.");
        }

	    foreach (var _hudPrf in hudPrfs)
	    {
	        var _hud = (GameObject) Instantiate(_hudPrf.gameObject);
	        _hud.transform.parent = transform;
	        _hud.transform.localPosition = Vector3.zero;
            _hud.transform.localScale = Vector3.one;
            m_HUDs.Add(_hud);
	    }

	    Game.Character.postCharacterDead += ListenCharacterDead;
	}

    void Destroy()
    {
        Game.Character.postCharacterDead -= ListenCharacterDead;
    }

    void ListenCharacterDead(Character _character)
    {
        if (!Game.Progress.IsState(GameProgress.State.OVER))
        {
            var _mode = Game.Mode as GameModeDeathMatch;
            if (_mode)
            {

            }
            else
            {
                
            }
        }
    }
}
