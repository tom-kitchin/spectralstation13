using Svelto.ECS;
using Components.Networking;

namespace Nodes.Networking
{
    public class ServerNode : NodeWithID
    {
        public IServerComponent serverComponent;
    }
}
