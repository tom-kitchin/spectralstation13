using UnityEngine;
using UnityEngine.Networking;
using Services.Networking;
using Components.Control;

namespace Implementers.Control
{
    [NetworkSettings(channel = (int)SpectreConnectionConfig.Channels.ReliableSequenced, sendInterval = 0.05f)]
    public class IsPlayerControllable : NetworkBehaviour, IIsPlayerControllableComponent
    {
        [SyncVar]
        public GameObject controllingPlayer;

        GameObject IIsPlayerControllableComponent.controllingPlayer { get { return controllingPlayer; } set { controllingPlayer = value; } }
    }
}
