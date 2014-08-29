using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIPopupList))]
public class UIMapSelector : MonoBehaviour
{
	public UIPopupList popupList;
	public UILabel label;

	void Start ()
	{
		if (popupList == null)
			popupList = GetComponent<UIPopupList>();

		popupList.onChange.Add(new EventDelegate(this, "OnChange"));

		Invoke("Refresh", 0.02f);
	}

	public void Refresh()
	{
		popupList.items.Clear();

		var _setting = Global.GameSetting();

		foreach (var _gameMap in _setting.maps)
			popupList.items.Add(_gameMap);

		popupList.value = _setting.maps[_setting.mapIdx];
	}

	void OnChange()
	{
		Global.GameSetting().map = popupList.value;
	}
}

