using UnityEngine;
using System.Collections;

public static class GameObjectHelper {

    public static IEnumerable GetChildRecursive(GameObject _go)
    {
        foreach ( Transform _child in _go.transform)
        {
            yield return _child.gameObject;
            foreach (var _grandChild in GetChildRecursive(_child.gameObject))
                yield return _grandChild;
        }
    }
 
}
