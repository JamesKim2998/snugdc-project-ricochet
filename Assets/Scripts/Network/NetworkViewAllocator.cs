using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(NetworkView))]
public class NetworkViewAllocator : MonoBehaviour
{
	struct AllocationJob 
	{
		private WeakReference m_NetworkView;
		public NetworkView networkView { 
			get { return m_NetworkView.Target as NetworkView;} 
			set { m_NetworkView = new WeakReference(value); }
		}

		public Action callback;
	}

	Dictionary<string, AllocationJob> mAllocateRequests;
	Dictionary<string, NetworkViewID> mAllocationBuffer;

	void Start ()
	{
		mAllocateRequests = new Dictionary<string, AllocationJob>();
		mAllocationBuffer = new Dictionary<string, NetworkViewID>();
	}

	void Update ()
	{

	}

	public void Allocate(string _requestKey, NetworkView _networkView)
	{
		Allocate(_requestKey, _networkView, null);
	}
	
	public void Allocate(string _requestKey, NetworkView _networkView, Action _callback)
	{
		NetworkViewID _bufferedViewID;
		if (mAllocationBuffer.TryGetValue(_requestKey, out _bufferedViewID)) 
		{
			_networkView.viewID = _bufferedViewID;
			return;
		}

		if (mAllocateRequests.ContainsKey(_requestKey))
		{
			Debug.Log("Request " + _requestKey + " is incomplete. Please wait.");
			return;
		}
		
		if (Network.isServer) 
		{
			_networkView.viewID = Network.AllocateViewID();
			mAllocationBuffer.Add(_requestKey, _networkView.viewID);
		}
		else if (Network.isClient)
		{
			var _job = new AllocationJob();
			_job.networkView = _networkView;
			_job.callback = _callback;
			mAllocateRequests.Add(_requestKey, _job);
			networkView.RPC("NetworkViewAllocator_RequestViewID", RPCMode.Server, Network.player, _requestKey);
		}
		else 
		{
			Debug.Log("The connect is not established yet, try after connected to server.");
			return;
		}
	}

	public void Cancel(string _requestKey)
	{
		if (mAllocateRequests.ContainsKey(_requestKey))
		{
			mAllocateRequests.Remove(_requestKey);
		}
	}

	[RPC]
	void NetworkViewAllocator_RequestViewID(NetworkPlayer _requestor, string _requestID)
	{
		NetworkViewID _viewID;

		if (mAllocationBuffer.ContainsKey(_requestID)) 
		{
			_viewID = mAllocationBuffer[_requestID];
		}
		else 
		{
			_viewID = Network.AllocateViewID();
			mAllocationBuffer.Add(_requestID, _viewID);
		}

		networkView.RPC("NetworkViewAllocator_ResponseViewID", _requestor, _viewID, _requestID);
	}

	[RPC]
	void NetworkViewAllocator_ResponseViewID(NetworkViewID _viewID, string _requestID)
	{
		AllocationJob _allocationJob;

		if (! mAllocateRequests.TryGetValue(_requestID, out _allocationJob))
		{
			Debug.Log("The request has been canceled.");
			return;
		}

		var _networkView = _allocationJob.networkView;
		if (_networkView != null)
		{
			mAllocationBuffer[_requestID] = _viewID;
			_networkView.viewID = _viewID;
			if (_allocationJob.callback != null)
				_allocationJob.callback();
		}
		else
		{
			Debug.Log("Network viewID is allocated but target view does not exist anymore.");
		}

		mAllocateRequests.Remove(_requestID);
	}
}

