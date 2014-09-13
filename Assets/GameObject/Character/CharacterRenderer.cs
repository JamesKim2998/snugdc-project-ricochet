using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterRenderer : MonoBehaviour
{
	public Material material;
    
	[HideInInspector]
	public List<SpriteRenderer> renderers;

	public SpriteRenderer head;
	public SpriteRenderer body;
	public SpriteRenderer mainArm;
	public SpriteRenderer subArm;
	public SpriteRenderer mainLeg;
	public SpriteRenderer subLeg;

    public CharacterRendererNetworkBrigde networkBrigde;

	private Character m_Character;
	private int m_PrevDirection;

	public void Awake()
	{
		renderers = new List<SpriteRenderer> {head, body, mainArm, subArm, mainLeg, subLeg};
	}

	public void Start()
	{
		m_Character = GetComponent<Character>();

		if (material != null)
		{
			foreach (var _renderer in renderers)
				_renderer.material = material;
		}
	}

	public void Update()
	{
		if (! m_Character) 
			return;

		if (m_Character.direction != m_PrevDirection )
		{
			m_PrevDirection = m_Character.direction;

			// note: normal을 정상으로 돌리기 위해서 두번 뒤집습니다.
			// 더 좋은 방법은 normal을 무시하도록 하는 것인데 ShaderLab을 몰라서 적용 못했습니다.
			foreach (var _renderer in renderers)
			{
				var _angles = _renderer.transform.localEulerAngles;
				_angles.y = m_PrevDirection > 0 ? 0 : 180;
				_renderer.transform.localEulerAngles = _angles;

				var _scale = _renderer.transform.localScale;
				_scale.x = m_PrevDirection > 0 ? 1 : -1;
				_renderer.transform.localScale = _scale;
			}
		}
	}

	public void SetColor(Color _color)
	{
		SetColorLocal (_color);
        if (networkBrigde)
            networkBrigde.RequestSetColor(_color);
	}

	public void SetColorLocal(Color _color)
	{
	    foreach (var _renderer in renderers
            .Where(_renderer => _renderer != null))
	    {
	        _renderer.color = _color;
	    }
	}
}

