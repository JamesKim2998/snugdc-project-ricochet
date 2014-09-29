using UnityEngine;
using System;
using System.Collections;
using Random = System.Random;

public class CharacterSpawner : MonoBehaviour
{
	public GameObject characterPrf;

	public Rect spawnRange;

    public Action<CharacterSpawner, Character> postSpawn;
	public Action<CharacterSpawner, Character> postDestroy;

	public float invinsibleTime = 1.0f;

    #region fx
    public GameObject fxSpawnPrf;
    public Vector2 fxSpawnOffset;
    #endregion

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
		var _position = Vector2.zero;
		_position.x = transform.position.x + spawnRange.xMin + UnityEngine.Random.Range(0, spawnRange.width);
		_position.y = transform.position.y + spawnRange.yMin + UnityEngine.Random.Range(0, spawnRange.height);
		return _position;
	}

	public Character Spawn() 
	{
		Vector3 _characterPosition = Locate();
	    var _characterID = new Random().Next();
        var _character = SpawnLocal(Global.Player().mine, _characterID, _characterPosition);

		var _destroyable = _character.GetComponent<Destroyable>();
		_destroyable.postDestroy += ListenDestroy;

		m_CharacterRef = new WeakReference( _character);

		if (networkView.enabled && Network.peerType != NetworkPeerType.Disconnected)
		{
			_character.networkView.viewID = Network.AllocateViewID();
			_character.networkView.enabled = true;
			networkView.RPC("CharacterSpawner_RequestSpawn", RPCMode.Others,
                            _character.networkView.viewID,
                            Network.player.guid, 
                            _characterID,
			                _characterPosition);
		}

		return _character;
	}

    Character SpawnLocal(PlayerInfo _playerInfo, int _characterID, Vector3 _position)
    {
//		Debug.Log("Spawn character local.");
        var _characterGO = (GameObject)Instantiate(characterPrf, _position, Quaternion.identity);
        var _character = _characterGO.GetComponent<Character>();

        _character.id = _characterID;
        _character.ownerPlayer = _playerInfo.guid;
        _character.type = _playerInfo.characterSelected;

        _character.hitEnabled = false;
        _character.Invoke("EnableHit", invinsibleTime);

        Database.Skin[_character.type].Apply(_character.renderer_);

        if (fxSpawnPrf)
        {
            var _fxSpawn = (GameObject) Instantiate(fxSpawnPrf);
            TransformHelper.SetParentWithoutScale(_fxSpawn, _character.gameObject);
            _fxSpawn.transform.position = _character.transform.position + (Vector3) fxSpawnOffset;
        }

        if (postSpawn != null) postSpawn(this, _character);

        return _character;
    }

	[RPC]
    void CharacterSpawner_RequestSpawn(NetworkViewID _viewID, string _player, int _characterID, Vector3 _position)
	{
//		Debug.Log("Spawn character network.");
        var _character = SpawnLocal(Global.Player()[_player], _characterID, _position);
		_character.networkView.enabled = true;
		_character.networkView.viewID = _viewID;
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

