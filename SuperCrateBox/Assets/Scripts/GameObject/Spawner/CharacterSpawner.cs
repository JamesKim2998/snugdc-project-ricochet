using UnityEngine;
using System;
using System.Collections;

public class CharacterSpawner : MonoBehaviour
{
	public GameObject characterPrf;

	public Rect spawnRange;

	public Action<CharacterSpawner, Character> postDestroy;

	public float invinsibleTime = 1.5f;

	private WeakReference m_CharacterRef;

	void OnDestroy()
	{
		Character _character = null;
		if (m_CharacterRef != null)
			_character = m_CharacterRef.Target as Character;

		if (_character != null)
		{
			var _destroyable = _character.gameObject.GetComponent<Destroyable>();
			if (_destroyable != null) _destroyable.postDestroy -= ListenDestroy;
		}
	}

	Vector2 Locate() 
	{
		Vector2 _position = Vector2.zero;
		_position.x = transform.position.x + spawnRange.xMin + UnityEngine.Random.Range(0, spawnRange.width);
		_position.y = transform.position.y + spawnRange.yMin + UnityEngine.Random.Range(0, spawnRange.height);
		return _position;
	}

	public Character Spawn() 
	{
		Vector3 _characterPosition = Locate();

		var _gameObj = GameObject.Instantiate(characterPrf, _characterPosition, Quaternion.identity) as GameObject;

		var _destroyable = _gameObj.GetComponent<Destroyable>();
		_destroyable.postDestroy += ListenDestroy;

//		Debug.Log("Spawn character local.");
		var _character = _gameObj.GetComponent<Character>();
		m_CharacterRef = new WeakReference( _character);
		_character.hitEnabled = false;
		_character.Invoke("EnableHit", invinsibleTime);
		_character.SendMessage("SetName", Global.Player().mine.name,SendMessageOptions.RequireReceiver);

		if (networkView.enabled && Network.peerType != NetworkPeerType.Disconnected)
		{
			_character.networkView.viewID = Network.AllocateViewID();
			_character.networkView.enabled = true;
			networkView.RPC("CharacterSpawner_RequestSpawn", RPCMode.Others, _character.networkView.viewID, _characterPosition, Global.Player ().mine.name);
		}

		return _character;
	}

	[RPC]
	void CharacterSpawner_RequestSpawn(NetworkViewID _viewID, Vector3 _position, string _name)
	{
//		Debug.Log("Spawn character network.");
		var _character = GameObject.Instantiate(characterPrf, _position, Quaternion.identity) as GameObject;
		_character.networkView.enabled = true;
		_character.networkView.viewID = _viewID;
		_character.SendMessage("SetName", _name, SendMessageOptions.RequireReceiver);
	}

	void ListenDestroy(Destroyable _destroyable)
	{
		m_CharacterRef = null;

		if (networkView != null && networkView.enabled) 
			Network.RemoveRPCs(networkView.viewID);

		if (postDestroy != null)
			postDestroy (this, _destroyable.GetComponent<Character>());
	}
}

