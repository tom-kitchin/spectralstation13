using UnityEngine;
using UnityEngine.Networking;
using Components.Networking;

namespace Implementers.Networking
{
    public class PlayerManager : MonoBehaviour, IPlayerComponent, INetworkEntityComponent
    {
        public string nickname;
        public int playerControllerId;
        public NetworkConnection connection;
        public NetworkIdentity identity;

        NetworkIdentity INetworkEntityComponent.identity { get { return identity; } }
        string IPlayerComponent.nickname { get { return nickname; } }
        int IPlayerComponent.playerControllerId { get { return playerControllerId; } }
        NetworkConnection IPlayerComponent.connection { get { return connection; } }
    }
}
