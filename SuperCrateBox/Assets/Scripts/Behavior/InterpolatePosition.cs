using UnityEngine;
using System.Collections;

public class InterpolatePosition : MonoBehaviour {

	public float weight = 0.9f;

	double m_LastSyncTimestamp = 0;
	Vector3 m_LastSyncPosition = Vector3.zero;
	Vector3 m_LastSyncVelocity = Vector3.zero;

	void OnSerializeNetworkView(BitStream _stream, NetworkMessageInfo _info)
	{
		Vector3 _syncPosition = Vector3.zero;
		Vector3 _syncVelocity = Vector3.zero;
		Vector3 _syncScale = Vector3.zero;

		if (_stream.isWriting)
		{
			_syncPosition = transform.position;
			_stream.Serialize(ref _syncPosition);
			
			_syncScale = transform.localScale;
			_stream.Serialize(ref _syncScale);

			_syncVelocity = rigidbody2D.velocity;
			_stream.Serialize(ref _syncVelocity);
		}
		else
		{
			if (m_LastSyncTimestamp > _info.timestamp) 
			{
				Debug.Log("serialization order cluttered!");
				return;
			}

			_stream.Serialize(ref _syncPosition);
			_stream.Serialize(ref _syncScale);
			_stream.Serialize(ref _syncVelocity);

//			Debug.Log(_syncPosition);
//			 Debug.Log(_syncVelocity);

			if (m_LastSyncPosition == Vector3.zero) 
				m_LastSyncPosition = _syncPosition;
			
			if (m_LastSyncVelocity == Vector3.zero) 
				m_LastSyncVelocity = _syncVelocity;

			float _syncDelay = (float) (Network.time - _info.timestamp);
			
			rigidbody2D.velocity = Vector3.Lerp(m_LastSyncVelocity, _syncVelocity, weight);
			m_LastSyncVelocity = rigidbody2D.velocity;
			
			transform.position = Vector3.Lerp(m_LastSyncPosition, _syncPosition + _syncDelay * m_LastSyncVelocity, weight);
			m_LastSyncPosition = transform.position;

			transform.localScale = _syncScale;
		}
	}
}
