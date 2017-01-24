using Svelto.ECS;
using Components.Motion;
using Components.Control;

namespace Nodes.Control
{
    public class PlayerControlledMovementNode : NodeWithID
    {
        public ICanMoveComponent canMoveComponent;
        public IMovementComponent movementComponent;
        public IIsPlayerControllableComponent isPlayerControllableComponent;
    }
}
