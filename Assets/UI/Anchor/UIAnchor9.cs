using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class UIAnchor9 : MonoBehaviour {
    public List<UIAnchor> anchors;

    public GameObject AddPrf(GameObject _prf, Anchor9 _anchor)
    {
        var _go = (GameObject) Instantiate(_prf);
        TransformHelper.SetParentWithoutScale(_go, anchors[(int) _anchor].gameObject);
        return _go;
    }
}
