using Svelto.ES;

namespace Components.Control
{
    public interface IIsPlayerControllableComponent : IComponent
    {
        bool currentlyControlled { get; set; }
    }
}
