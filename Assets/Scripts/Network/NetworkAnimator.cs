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
			networkView.RPC ("NetworkAnimator_SetTriggerRPC", RPCMode.Others, _trigger);
	}

	[RPC]
	void NetworkAnimator_SetTriggerRPC(string _trigger)
	{
		animator.SetTrigger (_trigger);
	}
	
	public void SetFloat(string _key, float _value)
	{
		animator.SetFloat (_key, _value);
		if (IsNetworkEnabled ())
			networkView.RPC ("NetworkAnimator_SetFloatRPC", RPCMode.Others, _key, _value);
	}

	[RPC]
	public void NetworkAnimator_SetFloatRPC(string _key, float _value)
	{
		animator.SetFloat ( _key, _value);
	}
}

