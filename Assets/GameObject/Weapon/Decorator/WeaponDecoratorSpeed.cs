
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class WeaponDecoratorSpeed : MonoBehaviour
{
    private Weapon m_Weapon;
	public float speed;

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
        _projectile.rigidbody2D.velocity = speed * _weapon.transform.right;
    }

}
