using UnityEngine;
using UnityEngine.Networking;
using Components.Networking;

namespace Implementers.Networking
{
    public class ClientManager : MonoBehaviour, IClientComponent, INetworkEntityComponent
    {
        public string nickname;
        public int playerControllerId;
        public NetworkConnection connection;
        public NetworkIdentity identity;

        NetworkIdentity INetworkEntityComponent.identity { get { return identity; } }
        string IClientComponent.nickname { get { return nickname; } }
        int IClientComponent.playerControllerId { get { return playerControllerId; } }
        NetworkConnection IClientComponent.connection { get { return connection; } }
    }
}
