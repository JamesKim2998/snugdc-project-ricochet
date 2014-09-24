using UnityEngine;
using System.Collections;

public class HUDBang : MonoBehaviour
{
    public UIGrid grid;
    public GameObject entityPrf;

    public float destroyDelay = 1;

    void Awake()
    {
        if (!grid) grid = GetComponent<UIGrid>();
    }

	void Start ()
	{
	    Game.Character.postCharacterDead += ListenCharacterDead;
	}

    void OnDestroy()
    {
	    Game.Character.postCharacterDead -= ListenCharacterDead;
    }

    void ListenCharacterDead(Character _character)
    {
        if (! HUDBangEntity.IsValidForRefresh(_character))
        {
            Debug.Log("Data is not valid for refresh. Ignore.");
            return;
        }

        var _entity = (GameObject) Instantiate(entityPrf);
        TransformHelper.SetParentWithoutScale(_entity, grid.gameObject);
        Destroy(_entity, destroyDelay);
        Invoke("Refresh", destroyDelay + 0.1f);

        var _bangEntity = _entity.GetComponent<HUDBangEntity>();
        _bangEntity.Refresh(_character);

        grid.Reposition();
    }

    void Refresh()
    {
        grid.Reposition();
    }
}
