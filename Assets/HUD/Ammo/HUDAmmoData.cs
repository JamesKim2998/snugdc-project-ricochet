using UnityEngine;
using System.Collections;

[System.Serializable]
public class HUDAmmoData : MonoBehaviour
{
    public int bucket;
    public Vector2 size;
    public Vector2 padding = new Vector2(5, 5);
    public int line = 1;
}
