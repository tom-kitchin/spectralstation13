using UnityEngine.Networking;

namespace Services.Networking
{
    /**
     * ConnectionConfig _must_ be the same for both client and server, so define it here for both to use.
     */
    public static class SpectreConnectionConfig
    {
        static ConnectionConfig _connectionConfig;

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
    }
}
