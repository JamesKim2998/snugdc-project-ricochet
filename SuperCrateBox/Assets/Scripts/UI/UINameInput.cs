using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UILabel))]
public class UINameInput : MonoBehaviour {

	public UILabel label;

	void Start()
	{
		label.text = Global.Player ().mine.name;
	}

	public void OnSubmit()
	{
		Debug.Log ("Name input " + label.text);
		Global.Player().mine.name = label.text;
	}
}
