using Svelto.ES;
using Components.Networking;

namespace Nodes.Networking
{
    public class NetworkMobNode : NodeWithID
    {
        INetworkEntityComponent networkEntityComponent;
        INetworkTransformComponent networkTransformComponent;
    }
}
