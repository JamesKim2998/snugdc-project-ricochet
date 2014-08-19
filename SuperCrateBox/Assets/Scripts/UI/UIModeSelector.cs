using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

[RequireComponent(typeof(UIPopupList))]
public class UIModeSelector : MonoBehaviour
{
	[System.Serializable]
	public class Mode
	{
		public GameModeType mode;
		public GameObject prefab;
	}

	public UIPopupList popupList;

	public GameObject modeParent;
	public List<Mode> modePrfs;
	private GameObject m_Mode;

	void Start ()
	{
		if ( modeParent == null
		    || modePrfs == null)
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
		
		foreach (var _modePrf in modePrfs)
			popupList.items.Add(_modePrf.mode.ToString());

		popupList.value = Global.GameSetting().modeSelected.mode.ToString();
	}
	
	void OnChange()
	{
		GameModeType _mode = (GameModeType) Enum.Parse(typeof(GameModeType), popupList.value);

		if (Enum.IsDefined(typeof(GameModeType), _mode))
			Global.GameSetting().modeSelected = Global.GameSetting().modes[_mode];

		var _setting = modePrfs.Find((_entity) => _entity.mode == _mode);

		if (_setting != null)
		{
			if (m_Mode != null)
				GameObject.Destroy(m_Mode);

			m_Mode = GameObject.Instantiate(_setting.prefab) as GameObject;
			m_Mode.transform.parent = modeParent.transform;
			m_Mode.transform.localPosition = Vector3.zero;
			m_Mode.transform.localScale = Vector3.one;
		}
		else 
		{
			Debug.LogError("The setting " + _mode.ToString() + " prefab is missing.");
		}

	}
}

