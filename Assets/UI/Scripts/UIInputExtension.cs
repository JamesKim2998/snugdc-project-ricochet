using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UIInput))]
public class UIInputExtension : MonoBehaviour {

	public UIInput target;

	public bool deselectOnSubmit = true;
	public AudioClip typingSound;
	public AudioClip submitSound;

	void Start ()
	{
	    if (!target) target = GetComponent<UIInput>();
		target.onChange.Add(new EventDelegate(this, "OnChange"));
		target.onSubmit.Add(new EventDelegate(this, "OnSubmit"));
	}

	void OnChange() 
	{
		if (typingSound != null)
            Global.SFX().PlayOneShot(typingSound);
	}

	void OnSubmit() 
	{
		if (submitSound != null)
            Global.SFX().PlayOneShot(submitSound);

		if (deselectOnSubmit)
			target.isSelected = false;
	}

}
