using UnityEngine.Networking;

namespace Components.Networking
{
    public interface IServerComponent : IComponent
    {
        NetworkManager manager { get; }
    }
}
