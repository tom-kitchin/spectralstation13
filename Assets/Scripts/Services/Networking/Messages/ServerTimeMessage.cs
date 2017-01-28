using UnityEngine;
using UnityEngine.Networking;

namespace Services.Networking.Messages
{
    public class ServerTimeMessage : MessageBase
    {
        public double timestamp;

        public ServerTimeMessage ()
        {
            timestamp = Network.time;
        }

        public ServerTimeMessage (double timestamp)
        {
            this.timestamp = timestamp;
        }
    }
}
