using UnityEngine;
using System.Collections;

public class HUDMinimapCharacter : MonoBehaviour
{
	void Start ()
	{
	    Game.Character.postCharacterAdded += ListenAdd;
    }

    void OnDestroy()
    {
        Game.Character.postCharacterAdded -= ListenAdd;
    }

    void Update()
    {
	
	}

    void ListenAdd(Character _character)
    {
        var _characterData = Database.Character[_character.type];
        if (!_characterData)
            return;

        var _gizmo = (GameObject) Instantiate(_characterData.minimapGizmo.gameObject);
        TransformHelper.SetParentWithoutScale(_gizmo, gameObject);

       var _characterGizmo = _gizmo.GetComponent<HUDMinimapCharacterGizmo>();
        _characterGizmo.character = _character;
    }

}
