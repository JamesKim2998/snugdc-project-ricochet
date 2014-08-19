using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(UIPopupList))]
public class UIModeSelector : MonoBehaviour
{
	[System.Serializable]
	public class Setting
	{
		public GameModeType mode;
		public GameObject prefab;
	}

	public UIPopupList popupList;

	public GameObject settingParent;
	public List<Setting> settingPrfs;
	public GameObject m_Setting;

	void Start ()
	{
		if ( settingParent == null
		    || settingPrfs == null)
		{
			Debug.LogError("Missing component!");
			return;
		}

		if (popupList == null)
			popupList = GetComponent<UIPopupList>();
		
		popupList.onChange.Add(new EventDelegate(this, "OnChange"));
		
		Refresh();
	}
	
	public void Refresh()
	{
		popupList.items.Clear();
		
		var _setting = Global.GameSetting();
		// new GameModeTestDef().TestLevel(_map);
		foreach (var _gameMode in EnumHelper.GetValues<GameModeType>())
			popupList.items.Add(_gameMode.ToString());

		popupList.value = GameModeType.TEST.ToString();
	}
	
	void OnChange()
	{
		GameModeType _mode = (GameModeType) Enum.Parse(typeof(GameModeType), popupList.value);

		if (Enum.IsDefined(typeof(GameModeType), _mode))
			Global.GameSetting().mode = GameMode.CreateDef(_mode);

		var _setting = settingPrfs.Find((_entity) => _entity.mode == _mode);

		if (_setting != null)
		{
			if (m_Setting != null)
				GameObject.Destroy(m_Setting);

			m_Setting = GameObject.Instantiate(_setting.prefab) as GameObject;
			m_Setting.transform.parent = settingParent.transform;
			m_Setting.transform.localPosition = Vector3.zero;
			m_Setting.transform.localScale = Vector3.one;
		}
		else 
		{
			Debug.LogError("The setting " + _mode.ToString() + " prefab is missing.");
		}

	}
}

