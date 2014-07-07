using UnityEngine;
using System.Collections;

public class Destroyable : MonoBehaviour {

	public delegate void PostDestroy(Destroyable _destroyable);
	public event PostDestroy postDestroy;

	void OnDestroy() {
		if (postDestroy != null) {
			postDestroy(this);
		}
	}

}
