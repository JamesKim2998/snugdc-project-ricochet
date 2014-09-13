using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour
{
    public WeaponType weapon;
    public bool empty { get { return weapon == WeaponType.NONE; } }

	public int score = 1;

	void Start() 
	{

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

}
