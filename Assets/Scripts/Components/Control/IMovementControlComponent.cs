using UnityEngine;
using Svelto.ECS;

namespace Components.Control
{
    public interface IMovementControlComponent : IComponent
    {
        bool active { get; set; }
        DispatchOnChange<Vector2> movementInput { get; set; }
        void CmdSetMovementInput (Vector2 newMovementInput);
    }
}
