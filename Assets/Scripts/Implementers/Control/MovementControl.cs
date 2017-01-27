using UnityEngine;
using UnityEngine.Networking;
using Svelto.ECS;
using Components.Control;

namespace Implementers.Control
{
    public class MovementControl : NetworkBehaviour, IMovementControlComponent
    {
        [SyncVar]
        public bool listening;

        Vector2 movementInput;
        
        
        [Command]
        void CmdSetMovementInput (Vector2 newMovementInput)
        {
            movementInput = newMovementInput;
        }

        bool IMovementControlComponent.listening { get { return listening; } set { listening = value; } }
        Vector2 IMovementControlComponent.movementInput { get { return movementInput; } set { movementInput = value; } }
        void IMovementControlComponent.CmdSetMovementInput(Vector2 newMovementInput) { CmdSetMovementInput(newMovementInput); }
    }
}
