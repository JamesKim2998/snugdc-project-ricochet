using UnityEngine;
using System.Collections;

public class CharacterRendererNetworkBrigde : MonoBehaviour
{
    public CharacterRenderer renderer_;

    public void RequestSetColor(Color _color)
    {
        if (Network.peerType != NetworkPeerType.Disconnected)
            networkView.RPC("CharacterRenderer_SetColor", RPCMode.Others, ColorHelper.ColorToVector(_color));
    }

    [RPC]
    void CharacterRenderer_SetColor(Vector3 _color)
    {
        renderer_.SetColorLocal(ColorHelper.VectorToColor(_color));
    }

}
