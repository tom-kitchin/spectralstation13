using UnityEngine;
using UnityEngine.Networking;
using Components.Networking;

namespace Implementers.Networking
{
    public class NetworkMob : MonoBehaviour, INetworkEntityComponent, INetworkTransformComponent
    {
        public NetworkIdentity identity;
        public NetworkTransform netTransform;

        NetworkIdentity INetworkEntityComponent.identity { get { return identity; } }
        NetworkTransform INetworkTransformComponent.netTransform { get { return netTransform; } }
    }
}
