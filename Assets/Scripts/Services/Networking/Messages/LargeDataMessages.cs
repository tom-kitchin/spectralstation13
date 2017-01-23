using System;
using UnityEngine.Networking;

namespace Services.Networking.Messages
{
    public class LargeDataPacketMessage : MessageBase
    {
        public uint packetNumber;
        public uint totalPackets;
        public uint size;
        public uint totalSize;
        public byte[] payload;

        public LargeDataPacketMessage () { }

        public LargeDataPacketMessage (uint packetNumber, uint totalPackets, uint size, uint totalSize, byte[] payload)
        {
            this.packetNumber = packetNumber;
            this.totalPackets = totalPackets;
            this.size = size;
            this.totalSize = totalSize;
            this.payload = payload;
        }
    }

    public class LargeDataInfoMessage : MessageBase
    {
        public uint totalPackets;
        public uint totalSize;

        public LargeDataInfoMessage () { }

        public LargeDataInfoMessage (uint totalPackets, uint totalSize)
        {
            this.totalPackets = totalPackets;
            this.totalSize = totalSize;
        }
    }

    public static class LargeDataHelper
    {
        public static LargeDataPacketMessage[] BreakDataIntoPackets(byte[] payload, int maxSize = 1024)
        {
            int totalSize = payload.Length;
            int numChunks = (int)Math.Ceiling(totalSize / (float)maxSize);
            LargeDataPacketMessage[] packets = new LargeDataPacketMessage[numChunks];

            // Iterate, creating packets for each maxSize chunk of the data.
            int payloadIndex = 0;
            byte[] packetData;
            for (int i = 0; i < numChunks - 1; i++)
            {
                packetData = new byte[maxSize];
                Array.Copy(payload, payloadIndex, packetData, 0, maxSize);
                packets[i] = new LargeDataPacketMessage((uint)i, (uint)numChunks, (uint)maxSize, (uint)totalSize, packetData);
                payloadIndex += maxSize;
            }

            // Sort out the last packet, which might be smaller than the max packet size.
            int finalPacketSize = totalSize - payloadIndex;
            packetData = new byte[finalPacketSize];
            Array.Copy(payload, payloadIndex, packetData, 0, finalPacketSize);
            packets[numChunks - 1] = new LargeDataPacketMessage((uint)numChunks - 1, (uint)numChunks, (uint)finalPacketSize, (uint)totalSize, packetData);

            return packets;
        }

        public static byte[] CombinePackets(LargeDataPacketMessage[] packets)
        {
            byte[] data = new byte[packets[0].totalSize];
            int outputIndex = 0;
            for (int i = 0; i < packets.Length; i++)
            {
                LargeDataPacketMessage packet = packets[i];
                Array.Copy(packet.payload, 0, data, outputIndex, packet.size);
                outputIndex += (int)packet.size;
            }
            return data;
        }
    }
}
