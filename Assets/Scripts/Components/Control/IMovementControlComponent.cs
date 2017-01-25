using UnityEngine;

namespace Components.Control
{
    public interface IMovementControlComponent : IComponent
    {
        bool listening { get; set; }
        Vector2 movementInput { get; set; }
        void CmdSetMovementInput (Vector2 newMovementInput);
    }
}
