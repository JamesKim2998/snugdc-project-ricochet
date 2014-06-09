using UnityEngine;
using System.Collections;

public class TerrainDetector : MonoBehaviour
{
	public delegate void PostDetect(Collider2D collision);

    public PostDetect postDetect;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Terrain")
        {
			if (Debug.isDebugBuild) {
				if (postDetect == null) {
					Debug.LogError(gameObject.name + " does not set terrain detector!");
					return;
				}
			}

			postDetect(collision);
        }
    }
}
   