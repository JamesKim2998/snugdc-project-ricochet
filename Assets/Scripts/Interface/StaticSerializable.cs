using UnityEngine;
using System.Collections;

public abstract class StaticSerializable : MonoBehaviour
{
	public abstract string Serialize();
	public abstract void Deserialize(string _data);
}

