using UnityEngine;
using System.Collections;

public class LayerDetector : MonoBehaviour
{
	public LayerMask layerMask;

	public delegate void PostDetect(Collider2D collision);
    public PostDetect postDetect;

    void OnTriggerEnter2D(Collider2D _collision)
    {
        if (LayerHelper.Exist(layerMask, _collision))
        {
			if (postDetect == null) 
			{
				Debug.LogError(gameObject.name + " does not set terrain detector!");
				return;
			}

			postDetect(_collision);
        }
    }
}
   