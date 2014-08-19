using UnityEngine;
using System.Collections;

public class UIMenuButton : MonoBehaviour 
{
	bool m_Selected = false;
	bool m_Hovering = false;

	public Sprite normal;
	public Sprite click;
	public Sprite over;

	UI2DSprite renderer;

	void Awake(){
		renderer = GetComponent<UI2DSprite>();
	}

	public void OnOver()
	{
		renderer.sprite2D = over;
	}

	public void OnIdle()
	{
		renderer.sprite2D = normal;
	}

	public void OnClick()
	{
		renderer.sprite2D = click;
	}
}
