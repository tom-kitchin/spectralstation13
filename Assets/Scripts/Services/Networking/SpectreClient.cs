using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using Config.Loaders.Helpers;

namespace Services.Networking
{
    public static class SpectreClient
    {
        public static NetworkClient networkClient;
        public static string networkAddress = "localhost";
        public static int networkPort = 7777;

        public static event StartClientHandler onStartClient;
        public static event ConfigDataHandler onConfigDataValidated;
        public static event OnMessageHandler onClientConnect;
        public static event OnMessageHandler onClientDisconnect;
        public static event OnMessageHandler onClientNotReady;
        public static event OnMessageHandler onClientError;
        public static event OnMessageHandler onClientScene;
        public static event OnMessageHandler onConfigChecksum;
        public static event OnMessageHandler onConfigData;

        static byte[] _checksum;
        static IFilesystemConfigHelper _filesystemHelper;
        static IFilesystemConfigHelper filesystemHelper {
            get {
                if (_filesystemHelper == null)
                {
                    _filesystemHelper = new WindowsFilesystemConfigHelper(null);
                }
                return _filesystemHelper;
            }
        }

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

            networkClient.RegisterHandler(SpectreMsgType.ConfigChecksum, OnConfigChecksum);
            networkClient.RegisterHandler(SpectreMsgType.ConfigData, OnConfigData);
        }

        /* SERVER MESSAGE HANDLERS */

        static void OnClientConnect (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnClientConnect");

            if (onClientConnect != null) { onClientConnect(netMsg); }

            // Might do some scene work here normally, but we don't need to worry about that yet.
            // Let's just declare that we're good to go.

            ClientScene.Ready(netMsg.conn);
        }

        static void OnClientDisconnect (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnClientDisconnect");

            if (onClientDisconnect != null) { onClientDisconnect(netMsg); }

            networkClient.Disconnect();
            networkClient.Shutdown();
            networkClient = null;
            ClientScene.DestroyAllClientObjects();
        }

        static void OnClientNotReady (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnClientNotReady");

            if (onClientNotReady != null) { onClientNotReady(netMsg); }
        }

        static void OnClientError (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnClientError");

            if (onClientError != null) { onClientError(netMsg); }

            ErrorMessage message = netMsg.ReadMessage<ErrorMessage>();
            Debug.Log("Error from server, error code " + message.errorCode);
        }

        static void OnClientScene (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnClientScene");

            if (onClientScene != null) { onClientScene(netMsg); }

            NetworkConnection conn = netMsg.conn;
            StringMessage message = netMsg.ReadMessage<StringMessage>();
            
            if (networkClient.isConnected && !NetworkServer.active)
            {
                // We'd usually change scene here, but we can worry about implementing that later.
                Debug.Log("Not yet implemented: Scene change to " + message.value);
            }
        }

        static void OnConfigChecksum (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnConfigChecksum");

            if (onConfigChecksum != null) { onConfigChecksum(netMsg); }
            
            _checksum = netMsg.reader.ReadBytesAndSize();
            string checksumString = System.Text.Encoding.UTF8.GetString(_checksum);
            File.Exists(filesystemHelper.GetMapCacheFilePath(checksumString)) {
                byte[] configData = File.ReadAllBytes(filesystemHelper.GetMapCacheFilePath(checksumString));
                if (ChecksumCheck(configData, _checksum))
                {
                    onConfigDataValidated(configData);
                    return;
                }
            }

            // If we didn't return early above, our config data isn't good or we don't have a cache for it,
            // so we need the data from the server.
            networkClient.Send(SpectreMsgType.RequestConfigData, new EmptyMessage());
        }

        static void OnConfigData (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnConfigData");

            if (onConfigData != null) { onConfigData(netMsg); }

            if (_checksum == null)
            {
                // We really shouldn't be receiving the data if we can't check it, whoops.
                // For the moment just ask for the checksum and start again from the beginning.
                networkClient.Send(SpectreMsgType.RequestConfigChecksum, new EmptyMessage());
                return;
            }
            
            byte[] configData = netMsg.reader.ReadBytesAndSize();
            if (ChecksumCheck(configData, _checksum))
            {
                SaveConfigCache(configData);
                onConfigDataValidated(configData);
                return;
            }

            // If we didn't return early above, our config data isn't good (corrupted in transit, presumably),
            // so we have to ask for it to be resent.
            networkClient.Send(SpectreMsgType.RequestConfigData, new EmptyMessage());
        }

        static bool ChecksumCheck(byte[] data, byte[] checksum)
        {
            SHA256 checksumHasher = SHA256Managed.Create();
            byte[] localChecksum = checksumHasher.ComputeHash(data);
            bool equal = false;
            if (checksum.Length == localChecksum.Length)
            {
                for (int i = 0; i < checksum.Length; i++)
                {
                    if (checksum[i] != localChecksum[i])
                    {
                        break;
                    }
                    equal = true;
                }
            }
            return equal;
        }

        static void SaveConfigCache(byte[] configData)
        {
            string checksumString = System.Text.Encoding.UTF8.GetString(_checksum);
            File.WriteAllBytes(filesystemHelper.GetMapCacheFilePath(checksumString), configData);
        }
    }

    public delegate GameObject StartClientHandler (NetworkClient client);
    public delegate void ConfigDataHandler (byte[] configData);
}