﻿using UnityEngine.Networking;

namespace Services.Networking
{
    public static class SpectreMsgType
    {
        public static short ConfigChecksum { get { return CustomMessage(1); } }
        public static short ConfigDataStart { get { return CustomMessage(2); } }
        public static short ConfigDataMiddle { get { return CustomMessage(3); } }
        public static short ConfigDataEnd { get { return CustomMessage(4); } }
        public static short RequestConfigChecksum { get { return CustomMessage(5); } }
        public static short RequestConfigData { get { return CustomMessage(6); } }

        static short CustomMessage(short num)
        {
            return (short)(MsgType.Highest + num);
        }
    }

    public delegate void OnMessageHandler (NetworkMessage netMsg);
}
