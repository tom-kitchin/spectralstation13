using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Datatypes.Networking;
using Components.Networking;

namespace Implementers.Networking
{
    // Channel 1: Unreliable
    [NetworkSettings(channel = 1, sendInterval = 0.05f)]
    public class NetworkPosition : NetworkBehaviour, INetworkPositionComponent
    {
        public SortedList<float, Position> positions;
        public Transform _transform;

        [SyncVar(hook = "UpdatePositions")]
        Position latestPositionBroadcast;

        [ClientCallback]
        void UpdatePositions (Position latestPosition)
        {
            positions.Add(latestPosition.timestamp, latestPositionBroadcast);
        }

        SortedList<float, Position> INetworkPositionComponent.positions { get { return positions; } }
        Transform INetworkPositionComponent.transform { get { return _transform; } }
    }
}
