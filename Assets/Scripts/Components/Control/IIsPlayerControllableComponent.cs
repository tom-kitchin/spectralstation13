using Svelto.ES;
using Implementers.Networking;

namespace Components.Control
{
    public interface IIsPlayerControllableComponent : IComponent
    {
        DispatcherOnChange<int, PlayerManager> controlledBy { get; set; }
    }
}
