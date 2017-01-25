using UnityEngine;
using UnityEngine.Networking;
using Components.Motion;

namespace Implementers.Motion
{
    public class CanMove : NetworkBehaviour, IMovementComponent, ICanMoveComponent
    {
        [SyncVar]
        public Vector2 movement;

        public float speed;
        public Transform _transform;

        float ICanMoveComponent.speed { get { return speed; } }
        Vector2 IMovementComponent.movement { get { return movement; } set { movement = value; } }
        Transform IMovementComponent.transform { get { return _transform; } }
    }
}
