using UnityEngine.Networking;
using Svelto.ES;

namespace Components.Networking
{
    public interface INetworkEntityComponent : IComponent
    {
        NetworkIdentity identity { get; }
    }
}
