using UnityEngine;
using Config;
using Datatypes.Networking;
using Implementers.Networking;

namespace Traits.Networking
{
    public class NetworkMobTrait : Trait
    {
        public string entityTypeIdentifier;

        public override void BuildAndAttach (ref GameObject go, ref WorldConfig config)
        {
            var networkPosition = go.AddComponent<NetworkPosition>();
            networkPosition._transform = go.transform;
            networkPosition.positions = new TimestampedList<Position>();
        }
    }
}
