using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using Config.Serializers;
using Services.Networking.Messages;

namespace Services.Networking
{
    public static class SpectreServer
    {
        public static int networkPort = 7777;
        public static int maxConnections = 8;

        public static Action onServerStart;
        public static event CreatePlayerHandler onCreatePlayer;
        public static event OnMessageHandler onServerConnect;
        public static event OnMessageHandler onServerDisconnect;
        public static event OnMessageHandler onServerReady;
        public static event OnMessageHandler onServerAddPlayer;
        public static event OnMessageHandler onServerRemovePlayer;
        public static event OnMessageHandler onServerError;
        public static event OnMessageHandler onRequestConfigChecksum;
        public static event OnMessageHandler onRequestConfigData;

        public static IConfigSerializer Serializer { set { _serializer = value; } }

        static IConfigSerializer _serializer;
        static byte[] _serializedConfig;
        static byte[] _configChecksum;

        /**
         * Let's get this party started.
         */
        public static void StartServer ()
        {
            LogFilter.currentLogLevel = LogFilter.Debug;
            ConfigureServer();
            if (!NetworkServer.Listen(networkPort))
            {
                Debug.LogError("Failed to start server on port " + networkPort.ToString());
                return;
            }
            RegisterServerMessages();
            Debug.Log("Started server on port " + networkPort.ToString());
            if (onServerStart != null) { onServerStart(); }
        }

        public static void Spawn (GameObject go)
        {
            NetworkServer.Spawn(go);
        }

        public static void SyncTimeToAll ()
        {
            NetworkServer.SendByChannelToAll(
                SpectreMsgType.SyncServerTime,
                new ServerTimeMessage(),
                (int)SpectreConnectionConfig.Channels.Unreliable
            );
        }

        static void ConfigureServer ()
        {
            Application.runInBackground = true;
            PrepareConfigForTransmission();
            ConnectionConfig config = SpectreConnectionConfig.connectionConfig;
            NetworkServer.Configure(config, maxConnections);
        }

        static void RegisterServerMessages ()
        {
            NetworkServer.RegisterHandler(MsgType.Connect, OnServerConnect);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnServerDisconnect);
            NetworkServer.RegisterHandler(MsgType.Ready, OnServerReady);
            NetworkServer.RegisterHandler(MsgType.AddPlayer, OnServerAddPlayer);
            NetworkServer.RegisterHandler(MsgType.RemovePlayer, OnServerRemovePlayer);
            NetworkServer.RegisterHandler(MsgType.Error, OnServerError);

            NetworkServer.RegisterHandler(SpectreMsgType.RequestConfigChecksum, OnRequestConfigChecksum);
            NetworkServer.RegisterHandler(SpectreMsgType.RequestConfigData, OnRequestConfigData);
        }

        static void SendBytesReliably (byte[] message, short msgType, NetworkConnection conn)
        {
            NetworkWriter writer = new NetworkWriter();
            writer.StartMessage(msgType);
            writer.WriteBytesFull(message);
            writer.FinishMessage();
            conn.SendWriter(writer, Channels.DefaultReliable);
        }

        static void SendLargeDataReliably (byte[] data, short startMsgType, short packetMsgType, short finishedMsgType, NetworkConnection conn)
        {
            LargeDataPacketMessage[] packets = LargeDataHelper.BreakDataIntoPackets(data);
            conn.Send(startMsgType, new LargeDataInfoMessage(packets[0].totalPackets, packets[0].totalSize));
            for (int i = 0; i < packets.Length; i++)
            {
                conn.Send(packetMsgType, packets[i]);
            }
            conn.Send(finishedMsgType, new LargeDataInfoMessage(packets[0].totalPackets, packets[0].totalSize));
        }

        static void PrepareConfigForTransmission ()
        {
            if (_serializer == null)
            {
                throw new NullReferenceException("Must set a serializer for config transmission handling.");
            }

            _serializedConfig = _serializer.Serialize();

            SHA256 checksumHasher = SHA256Managed.Create();
            _configChecksum = checksumHasher.ComputeHash(_serializedConfig);
        }

        /* SERVER MESSAGE HANDLERS */

        static void OnServerConnect (NetworkMessage netMsg)
        {
            Debug.Log("SpectreServer:OnServerConnect");

            if (onServerConnect != null) { onServerConnect(netMsg); }

            netMsg.conn.Send(SpectreMsgType.SyncServerTime, new ServerTimeMessage());
        }

        static void OnServerDisconnect (NetworkMessage netMsg)
        {
            Debug.Log("SpectreServer:OnServerDisconnect");

            if (onServerDisconnect != null) { onServerDisconnect(netMsg); }

            NetworkServer.DestroyPlayersForConnection(netMsg.conn);
        }

        static void OnServerReady (NetworkMessage netMsg)
        {
            Debug.Log("SpectreServer:OnServerReady");

            if (onServerReady != null) { onServerReady(netMsg); }

            NetworkServer.SetClientReady(netMsg.conn);
        }

        static void OnServerAddPlayer (NetworkMessage netMsg)
        {
            Debug.Log("SpectreServer:OnServerAddPlayer");

            if (onServerAddPlayer != null) { onServerAddPlayer(netMsg); }

            NetworkConnection conn = netMsg.conn;
            AddPlayerMessage message = netMsg.ReadMessage<AddPlayerMessage>();
            short playerControllerId = message.playerControllerId;
            if (playerControllerId < conn.playerControllers.Count && conn.playerControllers[playerControllerId].IsValid && conn.playerControllers[playerControllerId].gameObject != null)
            {
                Debug.LogError("There is already a player at playerControllerId " + playerControllerId + " for this connection.");
                return;
            }

            if (onCreatePlayer != null)
            {
                GameObject playerEntity = onCreatePlayer(conn, message);
                NetworkServer.AddPlayerForConnection(conn, playerEntity, playerControllerId);
            } else
            {
                Debug.LogError("No onCreatePlayer handler registered, unable to create player entity for playerControllerId " + playerControllerId);
                return;
            }
        }

        static void OnServerRemovePlayer (NetworkMessage netMsg)
        {
            Debug.Log("SpectreServer:OnServerRemovePlayer");

            if (onServerRemovePlayer != null) { onServerRemovePlayer(netMsg); }

            NetworkConnection conn = netMsg.conn;
            RemovePlayerMessage message = netMsg.ReadMessage<RemovePlayerMessage>();
            short playerControllerId = message.playerControllerId;
            PlayerController playerController = conn.playerControllers[playerControllerId];
            if (playerController.gameObject != null)
            {
                NetworkServer.Destroy(playerController.gameObject);
            }
            conn.playerControllers.RemoveAt(playerControllerId);
        }

        static void OnServerError (NetworkMessage netMsg)
        {
            Debug.Log("SpectreServer:OnServerError");

            if (onServerError != null) { onServerError(netMsg); }

            ErrorMessage message = netMsg.ReadMessage<ErrorMessage>();
            Debug.Log("Error from " + netMsg.conn.address + ", error code " + message.errorCode);
        }

        static void OnRequestConfigChecksum (NetworkMessage netMsg)
        {
            Debug.Log("SpectreServer:OnRequestConfigChecksum");

            if (onRequestConfigChecksum != null) { onRequestConfigChecksum(netMsg); }

            if (_configChecksum == null || _serializedConfig == null)
            {
                PrepareConfigForTransmission();
            }

            SendBytesReliably(_configChecksum, SpectreMsgType.ConfigChecksum, netMsg.conn);
        }

        static void OnRequestConfigData (NetworkMessage netMsg)
        {
            Debug.Log("SpectreServer:OnRequestConfigData");

            if (onRequestConfigData != null) { onRequestConfigData(netMsg); }

            if (_configChecksum == null || _serializedConfig == null)
            {
                PrepareConfigForTransmission();
            }
            SendLargeDataReliably(_serializedConfig, SpectreMsgType.ConfigDataStart, SpectreMsgType.ConfigDataPacket, SpectreMsgType.ConfigDataFinished, netMsg.conn);
        }
    }

    public delegate GameObject CreatePlayerHandler (NetworkConnection conn, AddPlayerMessage message);
}