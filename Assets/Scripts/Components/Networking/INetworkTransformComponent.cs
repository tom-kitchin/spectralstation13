using UnityEngine;
using UnityEngine.Networking;
using Svelto.ES;

namespace Components.Networking
{
    public interface INetworkTransformComponent : IComponent
    {
        NetworkTransform netTransform { get; }
    }
}
