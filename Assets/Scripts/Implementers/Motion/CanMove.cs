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

        // Grumble grumble this doesn't work because you can only trigger commands on things you own.
        // NOT THAT IT LOGS THIS ERROR it just bloody runs it locally. Jerks.
        [Command]
        public void CmdUpdateMovement (Vector2 newMovement) {
            movement = newMovement;
        }

        float ICanMoveComponent.speed { get { return speed; } }
        Vector2 IMovementComponent.movement { get { return movement; } set { movement = value; } }
        Transform IMovementComponent.transform { get { return _transform; } }
    }
}
