using UnityEngine;

namespace Components.Motion
{
    public interface IMovementComponent : IComponent
    {
        Vector2 movement { get; set; }
        Transform transform { get; }

        void CmdUpdateMovement (Vector2 newMovement);
    }
}
