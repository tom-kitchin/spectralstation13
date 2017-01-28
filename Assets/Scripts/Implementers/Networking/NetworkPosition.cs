using UnityEngine;
using UnityEngine.Networking;
using Datatypes.Networking;
using Components.Networking;
using Services.Networking;

namespace Implementers.Networking
{
    [NetworkSettings(channel = (int)SpectreConnectionConfig.Channels.Unreliable, sendInterval = 0.05f)]
    public class NetworkPosition : NetworkBehaviour, INetworkPositionComponent
    {
        public TimestampedList<Position> positions;
        public Transform _transform;

        [SyncVar(hook = "UpdatePositions")]
        public Position latestPositionBroadcast;

        [ClientCallback]
        void UpdatePositions (Position latestPosition)
        {
            try
            {
                positions.Add(latestPosition.timestamp, latestPosition);
                latestPositionBroadcast = latestPosition;
            } catch (System.ArgumentException)
            {
                Debug.LogError("Already have this key? Timestamp: " + latestPosition.timestamp.ToString() + ", position: " + latestPosition.position.ToString() + ", previous was: " + positions[latestPosition.timestamp].position.ToString() + ", server time is " + SpectreClient.serverTime.ToString());
            }
        }

        Position INetworkPositionComponent.latestPositionBroadcast { set { latestPositionBroadcast = value; } }
        TimestampedList<Position> INetworkPositionComponent.positions { get { return positions; } }
        Transform INetworkPositionComponent.transform { get { return _transform; } }
    }
}
