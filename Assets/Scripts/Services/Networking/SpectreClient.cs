using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Services.Networking
{
    public static class SpectreClient
    {
        public static NetworkClient networkClient;
        public static string networkAddress = "localhost";
        public static int networkPort = 7777;

        public static event StartClientHandler onStartClient;

        /**
         * Let's get this party started.
         */
        public static void StartClient ()
        {
            ConfigureClient();
            networkClient.Connect(networkAddress, networkPort);
            Debug.Log("Started client connection to " + networkAddress + ":" + networkPort);
            if (onStartClient != null) { onStartClient(networkClient); }
        }

        static void ConfigureClient ()
        {
            Application.runInBackground = true;
            networkClient = new NetworkClient();
            ConnectionConfig config = SpectreConnectionConfig.connectionConfig;
            networkClient.Configure(config, 1);
            RegisterClientMessages();
        }

        static void RegisterClientMessages ()
        {
            networkClient.RegisterHandler(MsgType.Connect, OnClientConnect);
            networkClient.RegisterHandler(MsgType.Disconnect, OnClientDisconnect);
            networkClient.RegisterHandler(MsgType.NotReady, OnClientNotReady);
            networkClient.RegisterHandler(MsgType.Error, OnClientError);
            networkClient.RegisterHandler(MsgType.Scene, OnClientScene);
        }

        /* SERVER MESSAGE HANDLERS */

        static void OnClientConnect (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnClientConnect");

            // Might do some scene work here normally, but we don't need to worry about that yet.
            // Let's just declare that we're good to go.

            ClientScene.Ready(netMsg.conn);
        }

        static void OnClientDisconnect (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnClientDisconnect");

            networkClient.Disconnect();
            networkClient.Shutdown();
            networkClient = null;
            ClientScene.DestroyAllClientObjects();
        }

        static void OnClientNotReady (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnClientNotReady");
        }

        static void OnClientError (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnClientError");

            ErrorMessage message = netMsg.ReadMessage<ErrorMessage>();
            Debug.Log("Error from server, error code " + message.errorCode);
        }

        static void OnClientScene (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnClientScene");

            NetworkConnection conn = netMsg.conn;
            StringMessage message = netMsg.ReadMessage<StringMessage>();
            
            if (networkClient.isConnected && !NetworkServer.active)
            {
                // We'd usually change scene here, but we can worry about implementing that later.
                Debug.Log("Not yet implemented: Scene change to " + message.value);
            }
        }
    }

    public delegate GameObject StartClientHandler (NetworkClient client);
}