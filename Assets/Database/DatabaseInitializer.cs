using UnityEngine;
using System.Collections;

public class DatabaseInitializer : MonoBehaviour
{
	public bool initializeOnStart = true;
	public bool destroyAfterInitialize = true;
	public bool overrideDef = false;

	private static bool m_Initialized = false;

	public DatabaseDef databaseDef;

	static DatabaseInitializer() {
		m_Initialized = false;
	}

	void Start ()
	{
		if (initializeOnStart)
			Initialize();
	}

	void Initialize() 
	{
		if (m_Initialized && ! overrideDef)
			return;

		Database.Instance.Apply(databaseDef);

		if (destroyAfterInitialize)
			Destroy( gameObject);
	}
}


