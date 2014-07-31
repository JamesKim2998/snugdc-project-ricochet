using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NetworkView))]
public class NetworkViewAllocator : MonoBehaviour
{
	Dictionary<string, WeakReference> mAllocateRequests;
	Dictionary<string, NetworkViewID> mAllocationCache;

	void Start ()
	{
		mAllocateRequests = new Dictionary<string, WeakReference>();
		mAllocationCache = new Dictionary<string, NetworkViewID>();
		if (Network.isClient) TryAssignSingletons();
	}

	void Update ()
	{

	}

	void OnServerInitialized()
	{
		TryAssignSingletons();
	}

	void OnConnectedToServer()
	{
		TryAssignSingletons();
	}

	void TryAssignSingletons()
	{
		if (Global.Instance.networkView.viewID == NetworkViewID.unassigned)
			AllocateNetworkViewID("Global", Global.Instance.networkView);

		if (Game.Instance.networkView.viewID == NetworkViewID.unassigned)
			AllocateNetworkViewID("Game", Game.Instance.networkView);
	}

	public void AllocateNetworkViewID(string _requestID, NetworkView _networkView)
	{
		if (mAllocateRequests.ContainsKey(_requestID))
		{
			Debug.Log("Request " + _requestID + " is incomplete. Please wait.");
			return;
		}

		if (mAllocationCache.ContainsKey(_requestID)) 
		{
			_networkView.viewID = mAllocationCache[_requestID];
			return;
		}

		if (Network.isServer) 
		{
			_networkView.viewID = Network.AllocateViewID();
			mAllocationCache.Add(_requestID, _networkView.viewID);
		}
		else if (Network.isClient)
		{
			mAllocateRequests.Add(_requestID, new WeakReference(_networkView));
			networkView.RPC("NetworkViewAllocator_RequestViewID", RPCMode.Server, Network.player, _requestID);
		}
		else 
		{
			Debug.Log("The connect is not established yet, try after connected to server.");
			return;
		}
	}

	[RPC]
	void NetworkViewAllocator_RequestViewID(NetworkPlayer _requestor, string _requestID)
	{
		NetworkViewID _viewID;

		if (mAllocationCache.ContainsKey(_requestID)) 
		{
			_viewID = mAllocationCache[_requestID];
		}
		else 
		{
			_viewID = Network.AllocateViewID();
			mAllocationCache.Add(_requestID, _viewID);
		}

		networkView.RPC("NetworkViewAllocator_ResponseViewID", _requestor, _viewID, _requestID);
	}

	[RPC]
	void NetworkViewAllocator_ResponseViewID(NetworkViewID _viewID, string _requestID)
	{
		var _networkViewRef = mAllocateRequests[_requestID];

		if (_networkViewRef == null)
		{
			Debug.LogWarning("Network view does not exist!");
		} 
		else
		{
			NetworkView _networkView = _networkViewRef.Target as NetworkView;
			if (_networkView != null)
			{
				_networkView.viewID = _viewID;
				mAllocationCache[_requestID] = _networkView.viewID;
			}
			else
			{
				Debug.Log("Network viewID is allocated but target view does not exist anymore.");
			}
		}

		mAllocateRequests.Remove(_requestID);
	}
}

