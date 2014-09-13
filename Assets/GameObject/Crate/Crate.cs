using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour 
{
	private string m_Weapon;
	public string weapon {
		get { return m_Weapon; }
		set { m_Weapon = value; }
	}

	public bool empty { get { return weapon == null; } }

	public int score = 1;

	void Start() 
	{

	}

	void DestroySelf() 
	{
		if (networkView.enabled)
		{
			if (networkView.isMine)
			{
				Network.Destroy(gameObject);
			}
		}
		else 
		{
			Destroy(gameObject);
		}
	}

	void OnCollisionEnter2D(Collision2D _collider)
	{
		if (_collider.gameObject.tag == "Character") 
		{
			// note: 자신이 생성한 캐릭터만 crate를 획득할 수 있습니다.
			var _character = _collider.gameObject.GetComponent<Character>();
			if (_character.ownerPlayer != Network.player.guid)
				return;

			var _detector = _collider.gameObject.GetComponent<CrateDetector>();

			if (_detector) 
			{
				if (! _detector.enabled) return;
				_detector.Obtain(this);
//				networkView.RPC("Crate_RequestObtain", RPCMode.All);

			    if (Network.peerType == NetworkPeerType.Disconnected)
                    Destroy(gameObject);
			    else
                    Network.Destroy(networkView.viewID);
			} 
			else 
			{
				Debug.Log("detector not found!");
			}
		}
	}

//	[RPC]
//	void Crate_RequestObtain()
//	{
//		DestroySelf();
//	}
}
