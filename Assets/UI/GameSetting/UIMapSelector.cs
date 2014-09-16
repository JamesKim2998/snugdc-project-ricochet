using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
using System;

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

		foreach (var _gameMap in Database.GameMap)
			popupList.items.Add(_gameMap.Value.name_);

	    popupList.value = Database.GameMap[_setting.map].name_;
	}

	void OnChange()
	{
	    var _theMapData = Database.GameMap.FirstOrDefault(_mapData => popupList.value == _mapData.Value.name_);
        
        if (_theMapData.Value != null)
			Global.GameSetting().map = _theMapData.Key;
		else 
			Debug.LogError("Scene " + popupList.value + " does not exist!");
	}
}

