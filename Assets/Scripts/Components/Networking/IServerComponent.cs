using UnityEngine.Networking;
using Svelto.ES;

namespace Components.Networking
{
    public interface IServerComponent : IComponent
    {
        NetworkManager manager { get; }
    }
}
