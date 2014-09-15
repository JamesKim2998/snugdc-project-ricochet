using UnityEngine;
using System.Collections;

public class UILogo : MonoBehaviour {

	public AudioClip hoverSound;

    private bool m_PlayAnimation = false;
    private float m_FadeTime = 0.5f;
	private float m_FadeElapsed = 0;

	void Start () 
	{
		if (GetComponent<AudioSource>() == null)
			gameObject.AddComponent<AudioSource>();

		audio.clip = hoverSound;
		audio.volume = 0;

		var _eventTrigger = GetComponent<UIEventTrigger>();
		if (_eventTrigger == null)
			_eventTrigger = gameObject.AddComponent<UIEventTrigger>();

		_eventTrigger.onHoverOver.Add(new EventDelegate(this, "OnHoverOver"));
		_eventTrigger.onHoverOut.Add(new EventDelegate(this, "OnHoverOut"));
	}
	
	void Update () 
	{
		bool _shouldChangeVolume = false;

		if (m_PlayAnimation)
		{
			if (m_FadeElapsed < m_FadeTime)
			{
				m_FadeElapsed += Time.deltaTime;
				_shouldChangeVolume = true;
			}
		}
		else
		{
			if ( m_FadeElapsed > 0 )
			{
				m_FadeElapsed -= Time.deltaTime;
				_shouldChangeVolume = true;
				
				if (m_FadeElapsed < 0)
					audio.Stop();
			}
		}

		if (_shouldChangeVolume)
		{
			float _volume = Mathf.Clamp01( m_FadeElapsed / m_FadeTime);
			audio.volume = _volume;
			Global.BGM().audio.volume = 1 - _volume;
		}
	}

	void PlayAnimation() 
	{
		if (m_PlayAnimation) 
			return;

		m_PlayAnimation = true;

		if ( ! audio.isPlaying) 
			audio.Play();

		GetComponent<UIPlayAnimation>().Play(true);

//		if (! animation.isPlaying)
//			animation.Play();
	}

	void OnHoverOver()
	{
		Invoke("PlayAnimation", 2.0f);
	}

	void OnHoverOut()
	{
		CancelInvoke("PlayAnimation");
		m_PlayAnimation = false;
		GetComponent<UIPlayAnimation>().Play(false);
	}
}
