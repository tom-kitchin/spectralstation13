﻿using UnityEngine;
using UnityEngine.Networking;
using Svelto.ECS;
using Implementers.Control;
using Components.Networking;
using Services.Networking;

namespace Implementers.Networking
{
	[NetworkSettings(channel = (int)SpectreConnectionConfig.Channels.ReliableSequenced, sendInterval = 0.05f)]
    public class PlayerManager : NetworkBehaviour, IPlayerComponent, INetworkEntityComponent
    {
        public string nickname;
        public NetworkConnection connection;
        public NetworkIdentity identity;

        [SyncVar(hook = "ChangeCurrentBody")]
        public GameObject currentBody;
        public DispatchOnChange<GameObject> currentBodyDispatcher;
        [ClientCallback]
        void ChangeCurrentBody (GameObject newBody)
        {
            currentBody = newBody;
            currentBodyDispatcher.value = newBody;
        }

        NetworkIdentity INetworkEntityComponent.identity { get { return identity; } }
        string IPlayerComponent.nickname { get { return nickname; } }
        int IPlayerComponent.playerControllerId { get { return playerControllerId; } }
        NetworkConnection IPlayerComponent.connection { get { return connection; } }
        GameObject IPlayerComponent.currentBody { get { return currentBody; } set { currentBody = value; currentBodyDispatcher.value = value; } }
        DispatchOnChange<GameObject> IPlayerComponent.currentBodyDispatcher { get { return currentBodyDispatcher; } }
        NetworkIdentity IPlayerComponent.identity { get { return identity; } }
        GameObject IPlayerComponent.manager { get { return gameObject; } }

        void Start ()
        {
            if (currentBodyDispatcher == null)
            {
                currentBodyDispatcher = new DispatchOnChange<GameObject>(GetInstanceID());
                currentBodyDispatcher.value = currentBody;
            }
        } 
    }
}
