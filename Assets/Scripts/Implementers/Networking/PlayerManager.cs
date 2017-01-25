using UnityEngine;
using UnityEngine.Networking;
using Components.Networking;

namespace Implementers.Networking
{
    public class PlayerManager : NetworkBehaviour, IPlayerComponent, INetworkEntityComponent
    {
        public string nickname;
        public NetworkConnection connection;
        public NetworkIdentity identity;

        [SyncVar]
        public GameObject currentBody;

        NetworkIdentity INetworkEntityComponent.identity { get { return identity; } }
        string IPlayerComponent.nickname { get { return nickname; } }
        int IPlayerComponent.playerControllerId { get { return playerControllerId; } }
        NetworkConnection IPlayerComponent.connection { get { return connection; } }
        GameObject IPlayerComponent.currentBody { get { return currentBody; } set { currentBody = value; } }
        NetworkIdentity IPlayerComponent.identity { get { return identity; } }
    }
}
