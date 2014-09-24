
using UnityEngine;

[RequireComponent(typeof(Weapon))]
public class WeaponDecoratorDrivingForce : MonoBehaviour
{
    private Weapon m_Weapon;
	public float drivingForce;
	public float explosionRadius;

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
        _projectile.GetComponent<Projectile>().drivingForce = drivingForce * _weapon.transform.right;
    }
}
