using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct CharacterSkinData
{
	[HideInInspector]
	public SpriteRenderer renderer;

	public Sprite sprite;
	public Vector2 position;
	public float angle;
	public Vector2 scale;
}

public class CharacterSkin : MonoBehaviour 
{
	public CharacterRenderers renderers;

	[HideInInspector]
	public List<CharacterSkinData> datas;

	public CharacterSkinData head;
	public CharacterSkinData body;
	public CharacterSkinData mainArm;
	public CharacterSkinData subArm;
	public CharacterSkinData mainLeg;
	public CharacterSkinData subLeg;

	public void Awake()
	{
		head.renderer = renderers.head;
		body.renderer = renderers.body;
		mainArm.renderer = renderers.mainArm;
		subArm.renderer = renderers.subArm;
		mainLeg.renderer = renderers.mainLeg;
		subLeg.renderer = renderers.subLeg;
	}

	public void Apply()
	{
		foreach (var _data in datas )
		{
			if (_data.renderer == null) continue;
			_data.renderer.sprite = _data.sprite;
			_data.renderer.transform.localPosition = _data.position;

			var _angle = _data.renderer.transform.localEulerAngles;
			_angle.z = _data.angle;
			_data.renderer.transform.localEulerAngles = _angle;

			_data.renderer.transform.localScale = _data.scale;
		}
	}
}
