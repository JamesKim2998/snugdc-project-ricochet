using UnityEngine;
using System.Collections;

public class EnableDontDestroyOnLoad : MonoBehaviour
{
	public bool applyOnStart = false;

	void Start()
	{
		if (applyOnStart)
			Execute();
	}

	public void Execute()
	{
		DontDestroyOnLoad(gameObject);
	}
}

