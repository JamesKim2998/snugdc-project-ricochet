using UnityEngine;
using System.Collections;

public static class LogCommon
{
	public static void MissingComponent() { Debug.LogError(LogMessages.MISSING_COMPONENT); }

}

