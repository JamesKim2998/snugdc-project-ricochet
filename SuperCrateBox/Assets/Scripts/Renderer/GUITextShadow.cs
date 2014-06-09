using UnityEngine;
using System.Collections;

public class GUITextShadow : MonoBehaviour {

	private GUIText m_TargetText;
	private GUIText m_ShadowText;

	// Use this for initialization
	void Start () {

		m_ShadowText = GetComponent<GUIText>();
		m_TargetText = transform.parent.GetComponent<GUIText>();

	}
	
	// Update is called once per frame
	void Update () {

		if (m_ShadowText.font != m_TargetText.font) {
			m_ShadowText.font = m_TargetText.font;
		}

		if (m_ShadowText.fontSize != m_TargetText.fontSize) {
			m_ShadowText.fontSize = m_TargetText.fontSize;
		}

		if (m_ShadowText.text != m_TargetText.text) {
			m_ShadowText.text = m_TargetText.text;
		}

		if (m_ShadowText.anchor != m_TargetText.anchor) {
			m_ShadowText.anchor = m_TargetText.anchor;
		}

		if (m_ShadowText.alignment != m_TargetText.alignment) {
			m_ShadowText.alignment = m_TargetText.alignment;
		}

	}
}
