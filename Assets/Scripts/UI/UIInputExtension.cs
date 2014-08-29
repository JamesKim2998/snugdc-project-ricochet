using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIInput))]
public class UIInputExtension : MonoBehaviour {

	public UIInput target;
	private AudioSource m_AudioSource;

	public bool deselectOnSubmit = true;
	public AudioClip typingSound;
	public AudioClip submitSound;

	void Start () 
	{
		if (target == null)
			target = GetComponent<UIInput>();

		if (target == null)
		{
			Debug.LogError("Target is not found!");
			return;
		}

		m_AudioSource = target.GetComponent<AudioSource>();
		if (m_AudioSource == null)
			m_AudioSource = target.gameObject.AddComponent<AudioSource>();

		target.onChange.Add(new EventDelegate(this, "OnChange"));
		target.onSubmit.Add(new EventDelegate(this, "OnSubmit"));
	}

	void OnChange() 
	{
		if (typingSound != null)
			m_AudioSource.PlayOneShot(typingSound);
	}

	void OnSubmit() 
	{
		if (submitSound != null)
			m_AudioSource.PlayOneShot( submitSound);

		if (deselectOnSubmit)
			target.isSelected = false;
	}

}
