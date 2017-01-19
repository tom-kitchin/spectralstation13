using System;
using Svelto.ES;
using Svelto.Factories;
using Services.Networking;
using Nodes.Networking;
using Config;

namespace Engines.Networking
{
    class ClientEngine : INodesEngine, IQueryableNodeEngine
    {
        public IEngineNodeDB nodesDB { set; private get; }

        readonly Type[] _acceptedNodes = {
            typeof(ClientNode)
        };
        IGameObjectFactory _factory;
        IEntityFactory _entityFactory;
        WorldConfig _config;

        public ClientEngine (IGameObjectFactory factory, IEntityFactory entityFactory, WorldConfig config, ref Action onSetupComplete)
        {
            _factory = factory;
            _entityFactory = entityFactory;
            _config = config;
            onSetupComplete += StartClient;
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

        void StartClient ()
        {
            SpectreClient.StartClient();
        }
    }
}
