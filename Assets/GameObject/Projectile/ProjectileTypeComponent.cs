using UnityEngine;
using System.Collections;

public class ProjectileTypeComponent : MonoBehaviour, IDatabaseTypeComponent<ProjectileType>
{
	public ProjectileType type;

    public ProjectileType Type()
    {
        return type;
    }
}

