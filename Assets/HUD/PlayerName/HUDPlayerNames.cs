using UnityEngine;
using System.Collections;

public class HUDPlayerNames : MonoBehaviour
{
    public HUDPlayerName playerNamePrf;

	void Start () {
        Game.Character.postCharacterAdded += ListenCharacterAdded;
	}

    void OnDestroy()
    {
        Game.Character.postCharacterAdded -= ListenCharacterAdded;
    }

    void ListenCharacterAdded(Character _character)
    {
        var _obj = (GameObject) Instantiate(playerNamePrf.gameObject);
        _obj.transform.parent = transform;
        _obj.transform.localPosition = Vector3.zero;
        _obj.transform.localScale = Vector3.one;

        var _playerName = _obj.GetComponent<HUDPlayerName>();
        _playerName.character = _character;
    }
}
