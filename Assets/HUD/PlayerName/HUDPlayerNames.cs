using UnityEngine;
using System.Collections;

public class HUDPlayerNames : MonoBehaviour
{
    public HUDAnchor playerNamePrf;

	void Start () {
        Game.Character.postCharacterAdded += ListenCharacterAdded;
	}

    void OnDestroy()
    {
        Game.Character.postCharacterAdded -= ListenCharacterAdded;
    }

    void ListenCharacterAdded(Character _character)
    {
        var _name = (GameObject) Instantiate(playerNamePrf.gameObject);
        _name.transform.parent = transform;
        _name.transform.localPosition = Vector3.zero;
        _name.transform.localScale = Vector3.one;
    }
}
