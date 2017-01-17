using UnityEngine;

namespace Components.Motion
{
    public interface ICanMoveComponent : IIdentifiedComponent
    {
        float speed { get; }
    }
}
