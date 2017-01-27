using UnityEngine.Networking;
using Svelto.Tasks;

namespace Services.Networking
{
    /**
     * ConnectionConfig _must_ be the same for both client and server, so define it here for both to use.
     */
    public static class SpectreConnectionConfig
    {
		public enum Channels {
			ReliableSequenced = 0,
			Unreliable = 1
		}

        public static ConnectionConfig connectionConfig {
            get {
                if (_connectionConfig == null)
                {
                    _connectionConfig = new ConnectionConfig();
                    _connectionConfig.Channels.Clear();
                    _connectionConfig.AddChannel(QosType.ReliableSequenced);
                    _connectionConfig.AddChannel(QosType.Unreliable);
                }
                return _connectionConfig;
            }
        }
        static ConnectionConfig _connectionConfig;

		public static float heartBeat = 0.05f;
        public static WaitForSecondsEnumerator heartbeatEnumerator {
            get {
                if (_heartbeatEnumerator == null)
                {
					_heartbeatEnumerator = new WaitForSecondsEnumerator(heartBeat);
                }
                return _heartbeatEnumerator;
            }
        }
        static WaitForSecondsEnumerator _heartbeatEnumerator;

		// How far behind server-canon we run the clients, in seconds. Helps keep things smooth.
		public static double clientDelay = 0.1d;
    }
}
