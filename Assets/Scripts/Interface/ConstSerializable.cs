using UnityEngine;
using System.Collections;

public abstract class ConstSerializable : MonoBehaviour
{
	public abstract string Serialize();
	public abstract void Deserialize(string _data);
}

