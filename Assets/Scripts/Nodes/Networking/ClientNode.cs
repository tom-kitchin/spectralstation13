using Svelto.ES;
using Components.Networking;

namespace Nodes.Networking
{
    public class ClientNode : NodeWithID
    {
        public IClientComponent clientComponent;
        public INetworkEntityComponent networkEntityComponent;
    }
}
