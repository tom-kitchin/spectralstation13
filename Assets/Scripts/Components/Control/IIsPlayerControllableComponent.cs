using Svelto.ECS;

namespace Components.Control
{
    public interface IIsPlayerControllableComponent : IComponent
    {
        DispatchOnChange<bool> currentlyControlled { get; }
        DispatchOnChange<int> controlledByNodeId { get; }
    }
}
