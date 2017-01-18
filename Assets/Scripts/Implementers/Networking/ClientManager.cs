using UnityEngine;
using UnityEngine.Networking;
using Components.Networking;

namespace Implementers.Networking
{
    public class ClientManager : MonoBehaviour, IClientComponent, INetworkEntityComponent
    {
        public string nickname;
        public NetworkConnection connection;
        public NetworkIdentity identity;

        NetworkIdentity INetworkEntityComponent.identity { get { return identity; } }
        string IClientComponent.nickname { get { return nickname; } }
        NetworkConnection IClientComponent.connection { get { return connection; } }
    }
}
