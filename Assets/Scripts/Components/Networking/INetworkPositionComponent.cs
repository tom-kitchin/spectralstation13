using UnityEngine;
using Datatypes.Networking;

namespace Components.Networking
{
    public interface INetworkPositionComponent : IComponent
    {
        Position latestPositionBroadcast { set; }
        TimestampedList<Position> positions { get; }
        Transform transform { get; }
    }
}
