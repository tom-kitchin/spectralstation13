using UnityEngine.Networking;

namespace Components.Networking
{
    public interface INetworkTransformComponent : IComponent
    {
        NetworkTransform netTransform { get; }
    }
}
