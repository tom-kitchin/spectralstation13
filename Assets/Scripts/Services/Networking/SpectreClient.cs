using System.IO;
using System.Text;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using Config.Loaders.Helpers;
using Services.Networking.Messages;

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
        public static event OnMessageHandler onConfigDataStart;
        public static event OnMessageHandler onConfigDataPacket;
        public static event OnMessageHandler onConfigDataFinished;
        public static event OnMessageHandler onSyncServerTime;

        public static double serverTime {
            get {
                return Network.time - _serverTimeDifference;
            }
        }

        static byte[] _checksum;
        static LargeDataPacketMessage[] _configLoadInProgressCollection;
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
        static double _serverTimeDifference;

        /**
         * Let's get this party started.
         */
        public static void StartClient ()
        {
            LogFilter.currentLogLevel = LogFilter.Debug;
            ConfigureClient();
            networkClient.Connect(networkAddress, networkPort);
            Debug.Log("Started client connection to " + networkAddress + ":" + networkPort);
            if (onStartClient != null) { onStartClient(networkClient); }
        }

        public static void Ready ()
        {
            // Adding a player also declares us Ready.
            ClientScene.AddPlayer(networkClient.connection, 0);
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
            networkClient.RegisterHandler(SpectreMsgType.ConfigDataStart, OnConfigDataStart);
            networkClient.RegisterHandler(SpectreMsgType.ConfigDataPacket, OnConfigDataPacket);
            networkClient.RegisterHandler(SpectreMsgType.ConfigDataFinished, OnConfigDataFinished);
            networkClient.RegisterHandler(SpectreMsgType.SyncServerTime, OnSyncServerTime);
        }


        static bool ChecksumCheckData (byte[] data, byte[] checksum)
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

        static void SaveConfigCache (byte[] configData)
        {
            if (!Directory.Exists(filesystemHelper.MapCacheDirectoryPath))
            {
                Directory.CreateDirectory(filesystemHelper.MapCacheDirectoryPath);
            }
            string checksumString = ChecksumToString(_checksum);
            File.WriteAllBytes(filesystemHelper.GetMapCacheFilePath(checksumString), configData);
        }

        static string ChecksumToString (byte[] checksum)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte b in checksum)
            {
                sb.Append(b.ToString("x2"));
            }
            return sb.ToString();
        }

        /* SERVER MESSAGE HANDLERS */

        static void OnClientConnect (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnClientConnect");

            if (onClientConnect != null) { onClientConnect(netMsg); }

            // We've connected, so we need the config checksum.
            networkClient.Send(SpectreMsgType.RequestConfigChecksum, new EmptyMessage());

            // Might do some scene work here normally, but we don't need to worry about that yet.
            // Let's just declare that we're good to go.

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
            string checksumString = ChecksumToString(_checksum);
            if (File.Exists(filesystemHelper.GetMapCacheFilePath(checksumString)))
            {
                byte[] configData = File.ReadAllBytes(filesystemHelper.GetMapCacheFilePath(checksumString));
                if (ChecksumCheckData(configData, _checksum))
                {
                    onConfigDataValidated(configData);
                    return;
                }
            }

            if (_configLoadInProgressCollection != null)
            {
                // We may be in the middle of loading the config, so let that finish.
                return;
            }

            // If we didn't return early above, our config data isn't good or we don't have a cache for it,
            // so we need the data from the server.
            networkClient.Send(SpectreMsgType.RequestConfigData, new EmptyMessage());
        }

        static void OnConfigDataStart (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnConfigDataStart");

            if (onConfigDataStart != null) { onConfigDataStart(netMsg); }

            if (_checksum == null)
            {
                // We really shouldn't be receiving the data if we can't check it, whoops.
                // We'll ask to restart when we get the ConfigDataFinished packet.
                Debug.Log("Received ConfigDataStart but don't have a checksum.");
                return;
            }

            LargeDataInfoMessage transferDetails = netMsg.ReadMessage<LargeDataInfoMessage>();
            _configLoadInProgressCollection = new LargeDataPacketMessage[transferDetails.totalPackets];
        }

        static void OnConfigDataPacket (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnConfigDataPacket");

            if (onConfigDataPacket != null) { onConfigDataPacket(netMsg); }

            LargeDataPacketMessage configPacket = netMsg.ReadMessage<LargeDataPacketMessage>();

            if (_configLoadInProgressCollection == null)
            {
                Debug.LogError("Received ConfigDataPacket but haven't received start instruction.");
                return;
            }

            if (_configLoadInProgressCollection[configPacket.packetNumber] != null)
            {
                Debug.LogError("Received ConfigDataPacket which has already been received, packet number " + configPacket.packetNumber);
                return;
            }

            _configLoadInProgressCollection[configPacket.packetNumber] = configPacket;
        }

        static void OnConfigDataFinished (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnConfigDataFinished");

            if (onConfigDataFinished != null) { onConfigDataFinished(netMsg); }

            if (_checksum == null)
            {
                // We really shouldn't be receiving the data if we can't check it, whoops.
                // For the moment just ask for the checksum again and start again.
                Debug.LogError("Received ConfigDataFinished but haven't received checksum.");
                _configLoadInProgressCollection = null;
                networkClient.Send(SpectreMsgType.RequestConfigChecksum, new EmptyMessage());
                return;
            }

            if (_configLoadInProgressCollection == null)
            {
                Debug.LogError("Received ConfigDataFinished but haven't received ConfigDataStart.");
                _configLoadInProgressCollection = null;
                networkClient.Send(SpectreMsgType.RequestConfigChecksum, new EmptyMessage());
                return;
            }

            LargeDataInfoMessage dataInfo = netMsg.ReadMessage<LargeDataInfoMessage>();

            bool missingPackets = false;
            for (int i = 0; i < dataInfo.totalPackets; i++)
            {
                if (_configLoadInProgressCollection[i] == null)
                {
                    Debug.LogError("Received ConfigDataFinished but packet " + i + " is missing.");
                    missingPackets = true;
                }
            }
            if (missingPackets)
            {
                _configLoadInProgressCollection = null;
                networkClient.Send(SpectreMsgType.RequestConfigChecksum, new EmptyMessage());
                return;
            }

            byte[] configData = LargeDataHelper.CombinePackets(_configLoadInProgressCollection);
            _configLoadInProgressCollection = null;

            if (!ChecksumCheckData(configData, _checksum))
            {
                // Either our config data or our checksum is corrupted,
                // so we should start from the beginning.
                Debug.LogError("Received ConfigDataFinished but output doesn't match checksum.");
                networkClient.Send(SpectreMsgType.RequestConfigChecksum, new EmptyMessage());
            }

            // Success!
            SaveConfigCache(configData);
            onConfigDataValidated(configData);
            return;
        }

        static void OnSyncServerTime (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnSyncServerTime");

            if (onSyncServerTime != null) { onSyncServerTime(netMsg); }

            ServerTimeMessage message = netMsg.ReadMessage<ServerTimeMessage>();
            double latency = networkClient.GetRTT() / 1000d; // ms to s
            _serverTimeDifference = Network.time - message.timestamp - latency;
            Debug.Log("NT:" + Network.time.ToString() + ", MT:" + message.timestamp.ToString() + ", LT:" + latency.ToString());
        }
    }

    public delegate GameObject StartClientHandler (NetworkClient client);
    public delegate void ConfigDataHandler (byte[] configData);
}