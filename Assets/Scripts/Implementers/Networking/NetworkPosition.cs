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
        Position latestPositionBroadcast;

        [ClientCallback]
        void UpdatePositions (Position latestPosition)
        {
            positions.Add(latestPosition.timestamp, latestPositionBroadcast);
        }

		Position INetworkPositionComponent.latestPositionBroadcast { set { latestPositionBroadcast = value; } }
		TimestampedList<Position> INetworkPositionComponent.positions { get { return positions; } }
        Transform INetworkPositionComponent.transform { get { return _transform; } }
    }
}
