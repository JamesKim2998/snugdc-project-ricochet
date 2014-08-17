using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIInput))]
public class UIInputAction : MonoBehaviour 
{
	public UIInput input;

	void Start()
	{
		if (input == null) 
			input = GetComponent<UIInput>();
	}

	public void Select()
	{
		input.isSelected = true;
	}

	public void Deselect() 
	{
		input.isSelected = false;
	}
}
