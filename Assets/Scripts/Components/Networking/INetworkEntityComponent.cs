using UnityEngine.Networking;

namespace Components.Networking
{
    public interface INetworkEntityComponent : IComponent
    {
        NetworkIdentity identity { get; }
    }
}
