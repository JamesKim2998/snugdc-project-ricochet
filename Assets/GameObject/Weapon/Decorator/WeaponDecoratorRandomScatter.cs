using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Weapon))]
public class WeaponDecoratorRandomScatter : MonoBehaviour
{
    private Weapon m_Weapon;
    public float deviation;

    void Awake()
    {
        m_Weapon = GetComponent<Weapon>();
        m_Weapon.doShoot += DoShoot;
    }

    void OnDestroy()
    {
        m_Weapon.doShoot -= DoShoot;
    }

    void DoShoot(Weapon _weapon, GameObject _projectile)
    {
        var _delta = (float)SimpleRNG.GetNormal(0, deviation);
        var _rotation = Quaternion.AngleAxis(_delta, Vector3.forward);
        var _velocity = _projectile.rigidbody2D.velocity;
        _velocity = _rotation * _velocity;
        _projectile.rigidbody2D.velocity = _velocity;
    }
}
