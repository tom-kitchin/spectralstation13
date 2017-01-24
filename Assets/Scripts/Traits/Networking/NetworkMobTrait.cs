using UnityEngine;
using UnityEngine.Networking;
using Config;

namespace Traits.Networking
{
    public class NetworkMobTrait : Trait
    {
        public string entityTypeIdentifier;

        public override void BuildAndAttach (ref GameObject go, ref WorldConfig config)
        {
            var networkIdentity = go.AddComponent<NetworkIdentity>();
            var networkTransform = go.AddComponent<NetworkTransform>();
            networkTransform.transformSyncMode = NetworkTransform.TransformSyncMode.SyncTransform;
            networkTransform.syncRotationAxis = NetworkTransform.AxisSyncMode.AxisZ;
            var mobImplementer = go.AddComponent<Implementers.Networking.NetworkMob>();
            mobImplementer.identity = networkIdentity;
            mobImplementer.netTransform = networkTransform;
        }
    }
}
