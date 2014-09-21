using UnityEngine;
using System.Collections;

public class HUDLayer : MonoBehaviour
{
    public Camera camera_;
    public UIAnchor9 anchor9;

    void Awake()
    {
        if (!camera_) camera_ = camera;
        if (!anchor9) anchor9 = GetComponent<UIAnchor9>();
        GameHUD.mainLayer = this;
    }
	
}
