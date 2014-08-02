using UnityEngine;
using System.Collections;

public class UIMenuButton : MonoBehaviour 
{
	bool m_Selected = false;
	bool m_Hovering = false;

	public UITweener normalTween;
	public UITweener selectedTween;
	public UITweener hoverTween;
	
	void Start () {
	
	}
	
	void Update () {
	
	}

	public void OnSelected()
	{
		if (m_Selected) return;
		m_Selected = true;
		Debug.Log("selected");
		normalTween.Play (false);
		selectedTween.Play (true);
	}

	public void OnDeselected()
	{
		if (! m_Selected) return;
		m_Selected = false;
		Debug.Log("deselected");
		normalTween.Play (true);
		selectedTween.Play (false);
	}

	public void OnHoverOver()
	{
		if (m_Hovering) return;
		m_Hovering = true;
		Debug.Log("hover over");
		if (! m_Selected)
		{
			normalTween.Play (false);
			hoverTween.Play (true);
		}
	}

	public void OnHoverOut()
	{
		if (! m_Hovering) return;
		m_Hovering = false;
		Debug.Log("hover out");
		if (m_Selected) 
		{
			normalTween.Play (true	);
			hoverTween.Play (false);
		}
	}
}
