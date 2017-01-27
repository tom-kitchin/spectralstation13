using Svelto.ECS;
using Components.Networking;

namespace Nodes.Networking
{
    public class NetworkMobNode : NodeWithID
    {
        public INetworkEntityComponent networkEntityComponent;
        public INetworkPositionComponent networkPositionComponent;
    }
}
