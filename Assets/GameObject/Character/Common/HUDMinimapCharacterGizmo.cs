using HutongGames.PlayMaker.Actions;
using UnityEngine;
using System.Collections;

class CharacterObserver : DestroyableObserver<Character> { }

public class HUDMinimapCharacterGizmo : MonoBehaviour
{
    private DestroyableObserver<Character> m_Observer;

    public Character character
    {
        get { return m_Observer.target; }
        set { m_Observer.target = value; }
    }

    void Awake()
    {
        m_Observer = gameObject.AddComponent<CharacterObserver>();
        m_Observer.postTargetDestroyed += delegate { Destroy(gameObject); };
    }

}
