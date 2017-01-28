using UnityEngine;
using UnityEngine.Networking;
using Svelto.ECS;

namespace Components.Networking
{
    public interface IPlayerComponent : IComponent
    {
        string nickname { get; }
        int playerControllerId { get; }
        NetworkConnection connection { get; }
        GameObject currentBody { get; set; }
        GameObject manager { get; }
        DispatchOnChange<GameObject> currentBodyDispatcher { get; }
        NetworkIdentity identity { get; }
    }
}
