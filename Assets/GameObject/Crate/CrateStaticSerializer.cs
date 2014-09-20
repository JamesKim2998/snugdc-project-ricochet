using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Collections;

[Serializable]
struct CrateStaticData
{
    public int id;
    public WeaponType weapon;
}

class CrateStaticSerializer : StaticSerializable
{
	private Crate m_Crate;

	public Crate crate 
    { 
		get { return m_Crate ?? (m_Crate = GetComponent<Crate>()); }
	}

	public override string Serialize ()
	{
	    var _data = new CrateStaticData
	    {
	        id = crate.AllocateID(),
	        weapon = crate.weapon,
	    };

        var _bf = new BinaryFormatter();
        var _os = new MemoryStream();
        _bf.Serialize(_os, _data);
		return Convert.ToBase64String (_os.GetBuffer ());
	}

	public override void Deserialize (string _serial64)
	{
        var _bf = new BinaryFormatter();
        var _serial = new MemoryStream(Convert.FromBase64String(_serial64));
        var _data = (CrateStaticData) _bf.Deserialize(_serial);
	    crate.AssignID(_data.id);
        crate.weapon = _data.weapon;
    }

}

