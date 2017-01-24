using UnityEngine;
using UnityEngine.Networking;
using Components.Networking;

namespace Implementers.Networking
{
    public class NetworkEntity : MonoBehaviour, INetworkEntityComponent
    {
        public NetworkIdentity identity;

        NetworkIdentity INetworkEntityComponent.identity { get { return identity; } }
    }
}
