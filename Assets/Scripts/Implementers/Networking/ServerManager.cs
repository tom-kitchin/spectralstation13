using UnityEngine;
using UnityEngine.Networking;
using Components.Networking;

namespace Implementers.Networking
{
    public class ServerManager : MonoBehaviour, IServerComponent
    {
        public NetworkManager manager;

        NetworkManager IServerComponent.manager { get { return manager; } }
    }
}
