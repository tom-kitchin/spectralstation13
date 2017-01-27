using UnityEngine;
using UnityEngine.Networking;
using Config;
using Implementers.Networking;

namespace Traits.Networking
{
    public class NetworkMobTrait : Trait
    {
        public string entityTypeIdentifier;

        public override void BuildAndAttach (ref GameObject go, ref WorldConfig config)
        {
            go.AddComponent<NetworkIdentity>();
            var networkPosition = go.AddComponent<NetworkPosition>();
            networkPosition._transform = go.transform;
        }
    }
}
