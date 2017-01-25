using Svelto.ECS;
using Components.Control;
using Components.Networking;

namespace Nodes.Control
{
    public class PlayerMovementControlNode : NodeWithID
    {
        public IPlayerComponent playerComponent;
        public IMovementControlComponent movementControlComponent;
    }
}
