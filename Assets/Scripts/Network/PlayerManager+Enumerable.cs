using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public partial class PlayerManager {

    public IEnumerator<KeyValuePair<string, PlayerInfo>> GetEnumerator()
    {
        return players.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return players.GetEnumerator();
    }
}
