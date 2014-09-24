using UnityEngine;
using System.Collections;

public class DisconnectFromServer : MonoBehaviour {

    public void Execute()
    {
        Network.Disconnect();
    }

}
