using Svelto.ECS;
using Components.Networking;

namespace Nodes.Networking
{
    public class NetworkEntityNode : NodeWithID
    {
        public INetworkEntityComponent networkEntityComponent;
    }
}
