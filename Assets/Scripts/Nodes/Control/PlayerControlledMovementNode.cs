using Svelto.ES;
using Components.Motion;
using Components.Control;

namespace Nodes.Control
{
    public class PlayerControlledMovementNode : NodeWithID
    {
        public IMovementComponent movementComponent;
        public IIsPlayerControllableComponent isPlayerControllableComponent;
    }
}
