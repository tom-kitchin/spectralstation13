using UnityEngine;
using UnityEngine.Networking;
using Components.Control;

namespace Implementers.Control
{
    public class IsPlayerControllable : NetworkBehaviour, IIsPlayerControllableComponent
    {
        [SyncVar]
        GameObject controllingPlayer;

        GameObject IIsPlayerControllableComponent.controllingPlayer { get { return controllingPlayer; } set { controllingPlayer = value; } }
    }
}
