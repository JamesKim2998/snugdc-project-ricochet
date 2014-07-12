using UnityEngine;
using System.Collections;

public class NetworkAnimator : MonoBehaviour
{
	[HideInInspector]
	public Animator animator;

	bool IsNetworkEnabled()
	{
		return networkView.isMine && (Network.peerType != NetworkPeerType.Disconnected);
	}

	void Awake()
	{
		animator = GetComponent<Animator>();
	}

	public void SetTrigger(string _trigger)
	{
		animator.SetTrigger (_trigger);
		
		if (IsNetworkEnabled ())
			networkView.RPC ("SetTriggerRPC", RPCMode.Others, _trigger);
	}

	[RPC]
	void SetTriggerRPC(string _trigger)
	{
		animator.SetTrigger (_trigger);
	}
	
	public void SetFloat(string _key, float _value)
	{
		animator.SetFloat (_key, _value);
		if (IsNetworkEnabled ())
			networkView.RPC ("SetFloatRPC", RPCMode.Others, _key, _value);
	}

	[RPC]
	public void SetFloatRPC(string _key, float _value)
	{
		animator.SetFloat ( _key, _value);
	}
}

