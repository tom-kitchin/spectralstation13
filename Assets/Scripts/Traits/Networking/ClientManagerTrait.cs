using UnityEngine;
using UnityEngine.Networking;
using Config;

namespace Traits.Networking
{
    public class ClientManagerTrait : Trait
    {
        public string nickname;
        public NetworkConnection connection;

        public override void BuildAndAttach (ref GameObject go, ref WorldConfig config)
        {
            var clientRepImplementer = go.AddComponent<Implementers.Networking.ClientManager>();
            clientRepImplementer.nickname = nickname;
            clientRepImplementer.connection = connection;
        }
    }
}
