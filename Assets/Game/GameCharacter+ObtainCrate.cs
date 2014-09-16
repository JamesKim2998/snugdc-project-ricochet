using UnityEngine;
using System.Collections;

public partial class GameCharacter {
    void ListenObtainCrate(Character _character, Crate _crate)
    {
        if (_character != character)
        {
            Debug.LogWarning("Character does not match. Ignore");
            return;
        }

        if (Network.peerType == NetworkPeerType.Disconnected)
            GameCharacter_ObtainCrate(Network.player.guid, _crate.id);
        else 
            networkView.RPC("GameCharacter_ObtainCrate", RPCMode.All,
                Network.player.guid, _crate.id);
    }
    
    [RPC]
    void GameCharacter_ObtainCrate(string _player, int _crateID)
    {
        Game.Statistic[_player].weaponPickUp.Add(_crateID);
    }

}
