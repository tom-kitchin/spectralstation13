using System;
using UnityEngine;
using UnityEngine.Networking;
using Svelto.ES;
using Svelto.Factories;
using Services.Networking;
using Nodes.Networking;
using Config;

namespace Engines.Networking
{
    class ServerEngine : INodesEngine, IQueryableNodeEngine
    {
        public IEngineNodeDB nodesDB { set; private get; }

        readonly Type[] _acceptedNodes = {
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
        }

        public void Remove (INode obj)
        {
        }

        void StartServer ()
        {
            SpectreServer.onCreatePlayer += ServerCreatePlayer;
            SpectreServer.StartServer();
        }

        GameObject ServerCreatePlayer (NetworkConnection conn, UnityEngine.Networking.NetworkSystem.AddPlayerMessage message)
        {
            Debug.Log("ServerEngine:ServerCreatePlayer");

            GameObject player = new GameObject();
            new Traits.Networking.ClientManagerTrait() {
                nickname = "test",
                connection = conn,
                playerControllerId = message.playerControllerId
            }.BuildAndAttach(ref player, ref _config);
            _entityFactory.BuildEntity(player.GetInstanceID(), player.GetComponent<IEntityDescriptorHolder>().BuildDescriptorType());
            Debug.Log(player);
            return player;
        }
    }
}
