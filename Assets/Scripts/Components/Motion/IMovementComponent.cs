using UnityEngine;

namespace Components.Motion
{
    public interface IMovementComponent : IIdentifiedComponent
    {
        Vector2 movement { get; set; }
    }
}
