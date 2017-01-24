using UnityEngine;
using UnityEngine.Networking;
using Config;

namespace Traits.Networking
{
    public class ServerManagerTrait : Trait
    {
        public override void BuildAndAttach (ref GameObject go, ref WorldConfig config)
        {
            var networkManager = go.AddComponent<NetworkManager>();
            networkManager.autoCreatePlayer = false;
            var serverManagerImpl = go.AddComponent<Implementers.Networking.ServerManager>();
            serverManagerImpl.manager = networkManager;
            go.AddComponent<NetworkManagerHUD>();
        }
    }
}
