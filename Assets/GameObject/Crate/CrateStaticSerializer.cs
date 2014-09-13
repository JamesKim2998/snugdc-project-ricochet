using UnityEngine;
using System.Collections;

public class CrateStaticSerializer : StaticSerializable
{
	private Crate m_Crate;

	public Crate crate 
    { 
		get { return m_Crate ?? (m_Crate = GetComponent<Crate>()); }
	}

	public override string Serialize ()
	{
		return ((int) crate.weapon).ToString();
	}

	public override void Deserialize (string _data)
	{
        crate.weapon = (WeaponType) int.Parse(_data);
	}

}

