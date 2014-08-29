using UnityEngine;
using System.Collections;

public class EnableOnContext : MonoBehaviour
{
	private ContextType m_Context = ContextType.NONE;
	public ContextType context {
		get { return m_Context; }
		set { 
			if (m_Context == value) return; 
			m_Context = value; 
			Refresh();
		}
	}

	void Start ()
	{
		Global.Context().postChanged += ListenContextChanged;
	}

	void OnDestroy()
	{
		Global.Context().postChanged -= ListenContextChanged;
	}

	void Refresh()
	{
		gameObject.SetActive(context == Global.Context());
	}

	void ListenContextChanged(ContextType _cur, ContextType _old)
	{
		context = _cur;
	}

}

