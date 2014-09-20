using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour
{
    private static readonly System.Random s_Random = new System.Random();

    public int id { get; private set; }

    public int AllocateID()
    {
        if (id != default(int)) return id;
        id = s_Random.Next();
        return id;
    }

    public void AssignID(int _id) { id = _id; }

    public WeaponType weapon;
    public bool empty { get { return weapon == WeaponType.NONE; } }

    void Awake()
    {}

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
