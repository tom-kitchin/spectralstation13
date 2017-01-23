using System;
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
        public static event OnMessageHandler onConfigDataMiddle;
        public static event OnMessageHandler onConfigDataEnd;

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

        public static void Ready ()
        {
            ClientScene.Ready(networkClient.connection);
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
            networkClient.RegisterHandler(SpectreMsgType.ConfigDataMiddle, OnConfigDataMiddle);
            networkClient.RegisterHandler(SpectreMsgType.ConfigDataEnd, OnConfigDataEnd);
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
            string checksumString = ChecksumToString(_checksum);
            if (File.Exists(filesystemHelper.GetMapCacheFilePath(checksumString))) {
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
                // For the moment just ask for the checksum and start again from the beginning.
                Debug.Log("Received start of config data but no checksum.");
                networkClient.Send(SpectreMsgType.RequestConfigChecksum, new EmptyMessage());
                return;
            }

            LargeDataPacketMessage configPacket = netMsg.ReadMessage<LargeDataPacketMessage>();
            _configLoadInProgressCollection = new LargeDataPacketMessage[configPacket.totalPackets];
            _configLoadInProgressCollection[configPacket.packetNumber] = configPacket;
        }

        static void OnConfigDataMiddle (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnConfigDataMiddle");

            if (onConfigDataMiddle != null) { onConfigDataMiddle(netMsg); }

            LargeDataPacketMessage configPacket = netMsg.ReadMessage<LargeDataPacketMessage>();

            if (_configLoadInProgressCollection == null)
            {
                Debug.Log("Received middle data packet but haven't received first data packet!");
                _configLoadInProgressCollection = new LargeDataPacketMessage[configPacket.totalPackets];
            }

            if (_configLoadInProgressCollection[configPacket.packetNumber] != null)
            {
                Debug.Log("Received a packet which has already been received, packet number " + configPacket.packetNumber);
                throw new ApplicationException("Received a packet which has already been received, packet number " + configPacket.packetNumber);
            }
            
            _configLoadInProgressCollection[configPacket.packetNumber] = configPacket;
        }

        static void OnConfigDataEnd (NetworkMessage netMsg)
        {
            Debug.Log("SpectreClient:OnConfigDataEnd");

            if (onConfigDataEnd != null) { onConfigDataEnd(netMsg); }

            if (_checksum == null)
            {
                // We really shouldn't be receiving the data if we can't check it, whoops.
                // For the moment just ask for the checksum again and start again.
                _configLoadInProgressCollection = null;
                networkClient.Send(SpectreMsgType.RequestConfigChecksum, new EmptyMessage());
                return;
            }
            
            LargeDataPacketMessage configPacket = netMsg.ReadMessage<LargeDataPacketMessage>();

            if (_configLoadInProgressCollection == null)
            {
                Debug.Log("Received end data packet but haven't received first data packet!");
                throw new ApplicationException("Received end data packet but haven't received first data packet!");
            }

            if (_configLoadInProgressCollection[configPacket.packetNumber] != null)
            {
                Debug.Log("Received a packet which has already been received, packet number " + configPacket.packetNumber);
                throw new ApplicationException("Received a packet which has already been received, packet number " + configPacket.packetNumber);
            }

            _configLoadInProgressCollection[configPacket.packetNumber] = configPacket;

            byte[] configData = LargeDataHelper.CombinePackets(_configLoadInProgressCollection);
            _configLoadInProgressCollection = null;
            
            if (ChecksumCheckData(configData, _checksum))
            {
                SaveConfigCache(configData);
                onConfigDataValidated(configData);
                return;
            }

            // If we didn't return early above, our config data isn't good (corrupted in transit, presumably),
            // so we have to ask for it to be resent.
            networkClient.Send(SpectreMsgType.RequestConfigData, new EmptyMessage());
        }

        static bool ChecksumCheckData(byte[] data, byte[] checksum)
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
            if (!Directory.Exists(filesystemHelper.MapCacheDirectoryPath)) {
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
    }

    public delegate GameObject StartClientHandler (NetworkClient client);
    public delegate void ConfigDataHandler (byte[] configData);
}