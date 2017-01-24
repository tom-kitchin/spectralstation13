using UnityEngine;
using UnityEngine.Networking;
using Config;

namespace Traits.Networking
{
    public class NetworkEntityTrait : Trait
    {
        public string entityTypeIdentifier;

        public override void BuildAndAttach (ref GameObject go, ref WorldConfig config)
        {
            var networkIdentity = go.AddComponent<NetworkIdentity>();
            var entityImplementer = go.AddComponent<Implementers.Networking.NetworkEntity>();
            entityImplementer.identity = networkIdentity;
        }
    }
}
