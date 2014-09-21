using UnityEngine;
using System.Collections;

class CharacterObserver : DestroyableObserver<Character> { }

public class HUDMinimapCharacterGizmo : MonoBehaviour
{
    private DestroyableObserver<Character> m_Observer;

    public UISprite icon;

    public Character character
    {
        get { return m_Observer.target; }
        set { m_Observer.target = value; }
    }

    void Awake()
    {
        m_Observer = gameObject.AddComponent<CharacterObserver>();
        m_Observer.postTargetDestroyed += delegate { Destroy(gameObject); };

        m_Observer.postTargetChanged += (_character, _old) =>
        {
            if (!_character)
            {
                icon.enabled = false;
                return;
            }

            icon.enabled = true;
            icon.spriteName = Database.Character[_character.type].minimapGizmoIcon;
        };
    }

    void Update()
    {
        if (character)
        {
            var _scale = icon.transform.localScale;
            if (_scale.x*character.direction < 0)
            {
                _scale.x *= -1;
                icon.transform.localScale = _scale;
            }
            
            var _angle = icon.transform.localEulerAngles;
            _angle.z = (character.aim - 90) * character.direction;
            icon.transform.localEulerAngles = _angle;
        }
    }
}
