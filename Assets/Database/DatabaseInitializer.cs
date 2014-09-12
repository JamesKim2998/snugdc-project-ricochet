using UnityEngine;
using System.Collections;

public class DatabaseInitializer : MonoBehaviour
{
	public bool initializeOnStart = true;
	public bool destroyAfterInitialize = true;
	public bool overrideDef = false;

	private static bool s_Initialized = false;

	public DatabaseDef databaseDef;

	void Start ()
	{
		if (initializeOnStart)
			Initialize();
	}

	void Initialize() 
	{
		if (s_Initialized && ! overrideDef)
			return;

        s_Initialized = true;

        Database.Instance.Apply(databaseDef);

		if (destroyAfterInitialize)
			Destroy( gameObject);
	}
}


