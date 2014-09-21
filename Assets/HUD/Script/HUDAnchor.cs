using UnityEngine;
using System.Collections;

public class HUDAnchor : MonoBehaviour
{
    public UIAnchor9 anchor9;
    public GameObject prefab;
    public Anchor9 anchor;
    public Vector2 offset;

    private GameObject m_HUD;

	void Start ()
	{
	    if (! anchor9) anchor9 = GameHUD.mainLayer.anchor9;
	    m_HUD = anchor9.AddPrf(prefab, anchor);
	    m_HUD.transform.localPosition = offset;
	}

    void Destroy()
    {
        Destroy(m_HUD.gameObject);
    }
}
