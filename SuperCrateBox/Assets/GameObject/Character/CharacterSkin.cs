using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CharacterSkinPart
{
	public Sprite sprite;
	public Vector2 position = Vector2.zero;
	public float angle = 0;
	public Vector2 scale = Vector2.one;
}

[System.Serializable]
public class CharacterSkin : MonoBehaviour 
{
	public CharacterSkinPart head;
	public CharacterSkinPart body;
	public CharacterSkinPart mainArm;
	public CharacterSkinPart subArm;
	public CharacterSkinPart mainLeg;
	public CharacterSkinPart subLeg;

	public void Awake()
	{
	}

	public void Start()
	{
	}

	public void Apply(CharacterRenderer _renderer)
	{
		var _datas = new List<CharacterSkinPart> {
			head,
			body,
			mainArm,
			subArm,
			mainLeg,
			subLeg,
		};

		Dictionary<CharacterSkinPart, SpriteRenderer> _rendererDictionary = new Dictionary<CharacterSkinPart, SpriteRenderer> {
			{ head, _renderer.head }, 
			{ body, _renderer.body }, 
			{ mainArm, _renderer.mainArm }, 
			{ subArm, _renderer.subArm }, 
			{ mainLeg, _renderer.mainLeg }, 
			{ subLeg, _renderer.subLeg }, 
		};

		foreach (var _data in _datas )
			Apply(_rendererDictionary[_data], _data);
	}

	static void Apply(SpriteRenderer _renderer, CharacterSkinPart _skin) {
		_renderer.sprite = _skin.sprite;
		_renderer.transform.localPosition = _skin.position;
		var _angle = _renderer.transform.localEulerAngles;
		_angle.z = _skin.angle;
		_renderer.transform.localEulerAngles = _angle;
		_renderer.transform.localScale = _skin.scale;
	}

}
