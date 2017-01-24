using UnityEngine;
using UnityEngine.Networking;
using Config;

namespace Traits.Networking
{
    public class PlayerManagerTrait : Trait
    {
        public string nickname;
        public int playerControllerId;
        public NetworkConnection connection;

        public override void BuildAndAttach (ref GameObject go, ref WorldConfig config)
        {
            var networkIdentity = go.AddComponent<NetworkIdentity>();
            var clientRepImplementer = go.AddComponent<Implementers.Networking.PlayerManager>();
            clientRepImplementer.nickname = nickname;
            clientRepImplementer.playerControllerId = playerControllerId;
            clientRepImplementer.connection = connection;
            clientRepImplementer.identity = networkIdentity;
        }
    }
}
