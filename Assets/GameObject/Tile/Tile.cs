using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileDatabase database;
    public TileDataKey data;

    public SpriteRenderer renderer_;
    public BoxCollider2D collider_;

    public GameObject prefab;

    void Awake()
    {
        Setup();
    }

    void Start()
    {
        Refresh();
    }

    public void Setup()
    {
        ComponentHelper.AssignComponentIfNotExists(gameObject, ref renderer_);
        ComponentHelper.AssignComponentIfNotExists(gameObject, ref collider_);
    }

    public void Refresh()
    {
        if (!database) return;
        var _tiledata = database[data];
        if (_tiledata == null) return;

        collider_.size = data.sizeWorld;
        renderer_.sprite = GenericHelper.SelectRandom(_tiledata.sprite);

        if (prefab)
        {
#if UNITY_EDITOR 
            DestroyImmediate(prefab);
#else
            Destroy(prefab);
#endif
            prefab = null;
        }

        if (_tiledata.prefab)
        {
            prefab = (GameObject)Instantiate(_tiledata.prefab);
            TransformHelper.SetParentLocal(prefab, gameObject);
            prefab.name = "prefab";
        }
    }
}
