using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using Svelto.ES;
using Svelto.Factories;
using Nodes.Networking;
using Config;

namespace Engines.Networking
{
    class ServerEngine : INodesEngine, IQueryableNodeEngine
    {
        public int networkPort = 7777;
        public int maxConnections = 8;
        NetworkServer _networkServer;

        public IEngineNodeDB nodesDB { set; private get; }

        readonly Type[] _acceptedNodes = {
            typeof(ServerNode),
            typeof(ClientNode)
        };
        IGameObjectFactory _factory;
        IEntityFactory _entityFactory;
        WorldConfig _config;
        ServerNode _serverNode;

        public ServerEngine (IGameObjectFactory factory, IEntityFactory entityFactory, WorldConfig config, ref Action onSetupComplete)
        {
            _factory = factory;
            _entityFactory = entityFactory;
            _config = config;
            onSetupComplete += StartServer;
        }

        public Type[] AcceptedNodes ()
        {
            return _acceptedNodes;
        }

        public void Add (INode obj)
        {
            if (obj is ServerNode)
            {
                _serverNode = obj as ServerNode;
            }
        }

        public void Remove (INode obj)
        {
            if (obj is ServerNode)
            {
                _serverNode = null;
            }
        }

        /**
         * Let's get this party started.
         */
        void StartServer ()
        {
            ConfigureServer();
            if (!NetworkServer.Listen(networkPort))
            {
                Debug.LogError("Failed to start server on port " + networkPort.ToString());
                return;
            }
            RegisterServerMessages();
            Debug.Log("Started server on port " + networkPort.ToString());
        }

        void ConfigureServer ()
        {
            Application.runInBackground = true;
            ConnectionConfig config = new ConnectionConfig();
            config.Channels.Clear();
            config.AddChannel(QosType.ReliableSequenced);
            config.AddChannel(QosType.Unreliable);
            NetworkServer.Configure(config, maxConnections);
        }

        void RegisterServerMessages ()
        {
            NetworkServer.RegisterHandler(MsgType.Connect, OnServerConnect);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnServerDisconnect);
            NetworkServer.RegisterHandler(MsgType.Ready, OnServerReady);
            NetworkServer.RegisterHandler(MsgType.AddPlayer, OnServerAddPlayer);
            NetworkServer.RegisterHandler(MsgType.RemovePlayer, OnServerRemovePlayer);
            NetworkServer.RegisterHandler(MsgType.Error, OnServerError);
        }

        /* SERVER MESSAGE HANDLERS */

        void OnServerConnect (NetworkMessage netMsg)
        {
            Debug.Log("ServerEngine:OnServerConnect");
        }

        void OnServerDisconnect (NetworkMessage netMsg)
        {
            Debug.Log("ServerEngine:OnServerDisconnect");

            NetworkServer.DestroyPlayersForConnection(netMsg.conn);
        }

        void OnServerReady (NetworkMessage netMsg)
        {
            Debug.Log("ServerEngine:OnServerReady");

            NetworkServer.SetClientReady(netMsg.conn);
        }

        void OnServerAddPlayer (NetworkMessage netMsg)
        {
            Debug.Log("ServerEngine:OnServerAddPlayer");

            NetworkConnection conn = netMsg.conn;
            AddPlayerMessage message = netMsg.ReadMessage<AddPlayerMessage>();
            short playerControllerId = message.playerControllerId;
            if (playerControllerId < conn.playerControllers.Count && conn.playerControllers[playerControllerId].IsValid && conn.playerControllers[playerControllerId].gameObject != null)
            {
                Debug.LogError("There is already a player at playerControllerId " + playerControllerId + " for this connection.");
                return;
            }
            GameObject playerEntity = new GameObject("Player " + playerControllerId);
            new Traits.Networking.ClientManagerTrait() {
                nickname = "test",
                connection = conn
            }.BuildAndAttach(ref playerEntity, ref _config);
            _entityFactory.BuildEntity(playerEntity.GetInstanceID(), playerEntity.GetComponent<IEntityDescriptorHolder>().BuildDescriptorType());

            NetworkServer.AddPlayerForConnection(conn, playerEntity, playerControllerId);
        }

        void OnServerRemovePlayer (NetworkMessage netMsg)
        {
            Debug.Log("ServerEngine:OnServerRemovePlayer");

            NetworkConnection conn = netMsg.conn;
            RemovePlayerMessage message = netMsg.ReadMessage<RemovePlayerMessage>();
            short playerControllerId = message.playerControllerId;
            PlayerController playerController = conn.playerControllers[playerControllerId];
            if (playerController.gameObject != null)
            {
                NetworkServer.Destroy(playerController.gameObject);
            }
            conn.playerControllers.RemoveAt(playerControllerId);
        }

        void OnServerError (NetworkMessage netMsg)
        {
            Debug.Log("ServerEngine:OnServerError");

            ErrorMessage message = netMsg.ReadMessage<ErrorMessage>();
            Debug.Log("Error from " + netMsg.conn.address + ", error code " + message.errorCode);
        }
    }
}
