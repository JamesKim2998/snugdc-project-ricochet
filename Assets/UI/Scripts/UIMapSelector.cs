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

		foreach (var _gameMap in SceneInfos.gameMaps)
			popupList.items.Add(_gameMap.name);

		popupList.value = SceneInfos.GameMap(_setting.mapIdx).name;
	}

	void OnChange()
	{
		Scene _scene;
		if (EnumHelper.TryParse(popupList.value, out _scene))
			Global.GameSetting().map = SceneInfos.Get(_scene).scene;
		else 
			Debug.LogError("Scene " + popupList.value + " does not exist!");
	}
}

