using UnityEngine;
using System.Collections;

public class CrateConstSerializer : ConstSerializable
{
	Crate m_Crate;

	public Crate crate { 
		get {
			if (m_Crate == null) m_Crate = GetComponent<Crate>();
			return m_Crate;
		} 
	}

	public override string Serialize ()
	{
		return crate.weapon;
	}

	public override void Deserialize (string _data)
	{
		crate.weapon = _data;
	}

}

