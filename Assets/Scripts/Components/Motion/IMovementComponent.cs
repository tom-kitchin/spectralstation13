using UnityEngine;
using Svelto.ES;

namespace Components.Motion
{
    public interface IMovementComponent : IComponent
    {
        Vector2 movement { get; set; }
        Transform transform { get; }
    }
}
