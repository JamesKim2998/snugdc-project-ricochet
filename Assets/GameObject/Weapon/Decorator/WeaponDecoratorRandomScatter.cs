﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Weapon))]
public class WeaponDecoratorRandomScatter : MonoBehaviour
{
    private Weapon m_Weapon;
    public Vector2 positionDeviation;
    public float directionDeviation;

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
        var _deltaPos = Vector2.zero;

        if (! Mathf.Approximately(positionDeviation.x, 0))
            _deltaPos.x = (float)SimpleRNG.GetNormal(0, positionDeviation.x);

        if (!Mathf.Approximately(positionDeviation.y, 0))
            _deltaPos.y = (float)SimpleRNG.GetNormal(0, positionDeviation.y);

        _projectile.transform.Translate(_deltaPos);


        if (! Mathf.Approximately(directionDeviation, 0))
        {
            var _delta = (float)SimpleRNG.GetNormal(0, directionDeviation);
            var _rotation = Quaternion.AngleAxis(_delta, Vector3.forward);
            var _velocity = _projectile.rigidbody2D.velocity;
            _velocity = _rotation * _velocity;
            _projectile.rigidbody2D.velocity = _velocity;
        }
    }
}
