using UnityEngine;
using System.Collections;

public class HUDLayer : MonoBehaviour
{

    public UIAnchor9 anchor9;

    void Awake()
    {
        if (!anchor9) anchor9 = GetComponent<UIAnchor9>();
        GameHUD.mainLayer = this;
    }
	
}
