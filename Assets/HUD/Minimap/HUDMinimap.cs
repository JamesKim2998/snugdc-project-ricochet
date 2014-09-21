using System.Collections.Generic;
using System.Linq;
using HutongGames.PlayMaker.Actions;
using UnityEditor;
using UnityEngine;
using System.Collections;

public class HUDMinimap : MonoBehaviour
{
    public Rect mapRect;
    public Vector2 containerSize;

    public readonly List<GameObject> gizmos = new List<GameObject>();

    private HUDMinimapCharacter m_Character;

    void Awake()
    {
        var _scale = Mathf.Min(containerSize.x/mapRect.size.x, containerSize.y/mapRect.size.y);
        transform.localScale = new Vector3(_scale, _scale, 1);

        var _origin = -mapRect.min - mapRect.size / 2;

        var _characters = new GameObject("characters");
        TransformHelper.SetParentWithoutScale(_characters, gameObject);
        _characters.transform.localPosition = _origin;
        m_Character = _characters.AddComponent<HUDMinimapCharacter>();
    }

    public GameObject Add(GameObject _gizmoPrf)
    {
        var _gizmo = (GameObject) Instantiate(_gizmoPrf);
        TransformHelper.SetParentWithoutScale(_gizmo, gameObject);
        gizmos.Add(_gizmo);
        return _gizmo;
    }

    public void Remove(GameObject _gizmo)
    {
        if (! gizmos.Remove(_gizmo))
            Debug.LogWarning("Gizmo does not exist! Continue anyway.");
        Destroy(_gizmo);
    }
}
