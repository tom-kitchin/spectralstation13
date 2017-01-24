using Svelto.ES;
using Components.Networking;

namespace Nodes.Networking
{
    public class PlayerNode : NodeWithID
    {
        public IPlayerComponent clientComponent;
        public INetworkEntityComponent networkEntityComponent;
    }
}
