using UnityEngine;
using UnityEngine.Networking;

namespace Services.Networking.Messages
{
    public class MovementInputMessage : MessageBase
    {
        public double timestamp;
        public Vector2 movementInput;
        public Vector2 endPosition;

        public MovementInputMessage () { }

        public MovementInputMessage (double timestamp, Vector2 movementInput, Vector2 endPosition)
        {
            this.timestamp = timestamp;
            this.movementInput = movementInput;
            this.endPosition = endPosition;
        }
    }
}
