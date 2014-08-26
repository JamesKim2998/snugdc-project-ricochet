using UnityEngine;
using System.Collections;

public class CharacterName : MonoBehaviour {

	public TextMesh text;

	void SetName(string _name)
	{
		text.text = _name;
	}

	void LateUpdate()
	{
		GameObject _textGameObject = text.gameObject;
		_textGameObject.transform.rotation = Quaternion.identity;
	}
}
