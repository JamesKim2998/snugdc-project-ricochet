using System.Linq;
using HutongGames.PlayMaker.Actions;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HUDAmmo : MonoBehaviour
{
    public Vector2 weaponIconPosition;
    private GameObject m_WeaponIcon;

    public Vector2 ammoOffset;
    private readonly SortedDictionary<int, HUDAmmoBundle> m_Bundles 
        = new SortedDictionary<int, HUDAmmoBundle>(new IntGreater());

    private Character m_Character;
    public Character character
    {
        get { return m_Character;  }
        private set
        {
            if (character == value)
                return;

            if (m_Character)
                m_Character.postWeaponChanged -= ListenWeaponChanged;

            m_Character = value;

            if (m_Character == null) 
                return;

            weapon = m_Character.weapon;
            m_Character.postWeaponChanged += ListenWeaponChanged;
        }
    }


    private Weapon m_Weapon;
    public Weapon weapon
    { 
		get { return m_Weapon; } 
		set {
			if (m_Weapon == value)
				return;

			if (m_Weapon)
				m_Weapon.postShoot -= ListenShoot;

            if (m_WeaponIcon)
            {
                Destroy(m_WeaponIcon);
                m_WeaponIcon = null;
            }

            ammo = 0;

            m_Weapon = value;

		    if (! m_Weapon)
		        return;

		    var _weaponData = Database.Weapon[weapon.type];
            m_WeaponIcon = (GameObject) Instantiate(_weaponData.hudAmmoEquipPrf);
            TransformHelper.SetParentWithoutScale(m_WeaponIcon, gameObject);
            m_WeaponIcon.transform.localPosition = weaponIconPosition;
            
            ammo = m_Weapon.ammo;

            Clear();

		    foreach (var _data in _weaponData.hudAmmoDatas)
		    {
		        var _bundle = new GameObject("bundle_" + _data.bucket);
                TransformHelper.SetParentWithoutScale(_bundle, gameObject);
		        var _theBundle = _bundle.AddComponent<HUDAmmoBundle>();
		        _theBundle.ammoData = _data;
                m_Bundles.Add(_data.bucket, _theBundle);
		    }

            m_Weapon.postShoot += ListenShoot;
        }
	}

    private int m_Ammo = 0;
    public int ammo
    {
        get { return m_Ammo; }
        set
        {
            m_Ammo = value;
            var _ammoLeft = value;
            foreach (var _bundle in m_Bundles)
            {
                var _count = _ammoLeft / _bundle.Key;
                _ammoLeft -= _count * _bundle.Key;
                _bundle.Value.count = _count;
            }
            Reposition();
        }
    }

    void Start()
    {
        Game.Character.postCharacterChanged += ListenCharacterChanged;
    }

    void OnDestroy()
    {
        if (weapon) weapon = null;
        Game.Character.postCharacterChanged -= ListenCharacterChanged;
    }

    void Reposition()
    {
        var _offset = new Vector2(ammoOffset.x, ammoOffset.y);

        foreach (var _bundleKV in m_Bundles)
        {
            var _bundle = _bundleKV.Value;
            _bundle.transform.localPosition = _offset;
            _offset.x += _bundleKV.Value.GetNextColumnX();
        }
    }

    void Clear()
    {
        foreach (var _bundle in m_Bundles)
            Destroy(_bundle.Value);
        m_Bundles.Clear();
    }

    void ListenCharacterChanged(Character _character)
    {
        character = _character;
    }

	void ListenWeaponChanged(Character _character, Weapon _old)
	{
	    if (_character != character)
	    {
	        Debug.LogWarning("Character is not match. Ignore.");
	        return;
	    }

	    weapon = _character.weapon;
	}

	void ListenShoot(Weapon _weapon, Projectile _projectile)
	{
		ammo = _weapon.ammo;
	}
}