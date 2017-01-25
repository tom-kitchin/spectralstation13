using UnityEngine;
using UnityEngine.Networking;
using Svelto.ECS;
using Components.Control;

namespace Implementers.Control
{
    public class MovementControl : NetworkBehaviour, IMovementControlComponent
    {
        [SyncVar]
        public bool active;

        public DispatchOnChange<Vector2> movementInput;
        
        [Command]
        void CmdSetMovementInput (Vector2 newMovementInput)
        {
            Debug.Log("Received movement input update command " + newMovementInput.ToString());
            movementInput.value = newMovementInput;
        }

        private void Start ()
        {
            movementInput = new DispatchOnChange<Vector2>(gameObject.GetInstanceID());
        } 

        bool IMovementControlComponent.active { get { return active; } set { active = value; } }
        DispatchOnChange<Vector2> IMovementControlComponent.movementInput { get { return movementInput; } set { movementInput = value; } }
        void IMovementControlComponent.CmdSetMovementInput(Vector2 newMovementInput) { CmdSetMovementInput(newMovementInput); }
    }
}
