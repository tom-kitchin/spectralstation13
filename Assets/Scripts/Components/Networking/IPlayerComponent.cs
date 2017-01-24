using UnityEngine.Networking;
using Svelto.ES;

namespace Components.Networking
{
    public interface IPlayerComponent : IComponent
    {
        string nickname { get; }
        int playerControllerId { get; }
        NetworkConnection connection { get; }
    }
}
