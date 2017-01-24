using Svelto.ECS;
using Components.Networking;

namespace Nodes.Networking
{
    public class PlayerNode : NodeWithID
    {
        public IPlayerComponent clientComponent;
        public INetworkEntityComponent networkEntityComponent;
    }
}
