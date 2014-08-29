using UnityEngine;
using System.Collections;

public class UIPopupButton : MonoBehaviour
{
	public UIButton button;
	public GameObject parent;
	public GameObject popupPrf;

	void Start ()
	{
		if (button == null) 
			button = GetComponent<UIButton>();
		
		if (parent == null)
			parent = button.transform.parent.gameObject;

		button.onClick.Add(new EventDelegate(this, "_OnClick"));
	}

	void _OnClick()
	{
		if (popupPrf == null)
		{
			Debug.LogError("Popup is not specified!");
			return;
		}

		button.isEnabled = false;

		var _popup = Instantiate(popupPrf) as GameObject;
		var _scale = _popup.transform.localScale;
		_popup.transform.parent = parent.transform;
		_popup.transform.localScale = _scale;

		var _destroyable = _popup.GetComponent<Destroyable>();
		if (_destroyable == null)
			_destroyable = _popup.AddComponent<Destroyable>();
		_destroyable.postDestroy += _ => { button.isEnabled = true; };
	}
}

