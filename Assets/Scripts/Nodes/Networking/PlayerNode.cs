using Svelto.ECS;
using Components.Networking;

namespace Nodes.Networking
{
    public class PlayerNode : NodeWithID
    {
        public IPlayerComponent playerComponent;
        public INetworkEntityComponent networkEntityComponent;
    }
}
