using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CharacterRenderers : MonoBehaviour
{
	[HideInInspector]
	public List<SpriteRenderer> renderers;

	public SpriteRenderer head;
	public SpriteRenderer body;
	public SpriteRenderer mainArm;
	public SpriteRenderer subArm;
	public SpriteRenderer mainLeg;
	public SpriteRenderer subLeg;

	public void Awake()
	{
		renderers = new List<SpriteRenderer> ();
		renderers.Add (head);
		renderers.Add (body);
		renderers.Add (mainArm);
		renderers.Add (subArm);
		renderers.Add (mainLeg);
		renderers.Add (subLeg);
	}

	public void SetColor(Color _color)
	{
		SetColorLocal (_color);
		if (Network.peerType != NetworkPeerType.Disconnected)
			networkView.RPC ("SetColorRPC", RPCMode.OthersBuffered, ColorHelper.ColorToVector( _color));
	}

	void SetColorLocal(Color _color)
	{
		foreach (var _renderer in renderers )
		{
			if (_renderer == null) continue;
			_renderer.color = _color;
			Debug.Log(_color);
		}
	}

	[RPC]
	void SetColorRPC(Vector3 _color)
	{
		SetColorLocal (ColorHelper.VectorToColor(_color));
	}
}

