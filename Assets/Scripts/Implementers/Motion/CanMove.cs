using UnityEngine;
using Components.Motion;

namespace Implementers.Motion
{
    public class CanMove : MonoBehaviour, IMovementComponent, ICanMoveComponent
    {
        public float speed;
        public Vector2 movement;
        public Transform _transform;

        float ICanMoveComponent.speed { get { return speed; } }
        Vector2 IMovementComponent.movement { get { return movement; } set { movement = value; } }
        Transform IMovementComponent.transform { get { return _transform; } }
    }
}
