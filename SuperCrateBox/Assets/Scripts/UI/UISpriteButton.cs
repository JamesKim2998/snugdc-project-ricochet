using UnityEngine;
using System.Collections;

public class UISpriteButton : MonoBehaviour 
{
	bool m_Pressed = false;
	bool m_Hovering = false;

	public UI2DSprite normal;
	public UI2DSprite hover;
	public UI2DSprite pressed;

	void Start()
	{
		normal.enabled = true;
		hover.enabled = false;
		pressed.enabled = false;
	}

	public void Refresh()
	{
		if (m_Pressed) 
		{
			normal.enabled = false;
			hover.enabled = false;
			pressed.enabled = true;
		}
		else 
		{
			pressed.enabled = false;

			if (m_Hovering)
			{
				normal.enabled = false;
				hover.enabled = true;
			}
			else 
			{
				normal.enabled = true;
				hover.enabled = false;
			}
		}
	}

	public void OnPressed()
	{
		if (m_Pressed) return;
		m_Pressed = true;
		Refresh();
	}

	public void OnReleased()
	{
		if (! m_Pressed) return;
		m_Pressed = false;
		Refresh();
	}

	public void OnHoverOver()
	{
		if (m_Hovering) return;
		m_Hovering = true;
		Refresh();
	}

	public void OnHoverOut()
	{
		if (! m_Hovering) return;
		m_Hovering = false;
		Refresh();
	}
}
