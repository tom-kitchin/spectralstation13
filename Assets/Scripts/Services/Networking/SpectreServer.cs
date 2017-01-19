﻿using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

namespace Services.Networking
{
    public static class SpectreServer
    {
        public static int networkPort = 7777;
        public static int maxConnections = 8;
        
        public static event CreatePlayerHandler onCreatePlayer;

        /**
         * Let's get this party started.
         */
        public static void StartServer ()
        {
            ConfigureServer();
            if (!NetworkServer.Listen(networkPort))
            {
                Debug.LogError("Failed to start server on port " + networkPort.ToString());
                return;
            }
            RegisterServerMessages();
            Debug.Log("Started server on port " + networkPort.ToString());
        }

        static void ConfigureServer ()
        {
            Application.runInBackground = true;
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
        }

        /* SERVER MESSAGE HANDLERS */

        static void OnServerConnect (NetworkMessage netMsg)
        {
            Debug.Log("SpectreServer:OnServerConnect");
        }

        static void OnServerDisconnect (NetworkMessage netMsg)
        {
            Debug.Log("SpectreServer:OnServerDisconnect");

            NetworkServer.DestroyPlayersForConnection(netMsg.conn);
        }

        static void OnServerReady (NetworkMessage netMsg)
        {
            Debug.Log("SpectreServer:OnServerReady");

            NetworkServer.SetClientReady(netMsg.conn);
        }

        static void OnServerAddPlayer (NetworkMessage netMsg)
        {
            Debug.Log("SpectreServer:OnServerAddPlayer");

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

            ErrorMessage message = netMsg.ReadMessage<ErrorMessage>();
            Debug.Log("Error from " + netMsg.conn.address + ", error code " + message.errorCode);
        }
    }

    public delegate GameObject CreatePlayerHandler (NetworkConnection conn, AddPlayerMessage message);
}