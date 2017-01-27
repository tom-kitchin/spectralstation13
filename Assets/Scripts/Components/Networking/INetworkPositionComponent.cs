using System.Collections.Generic;
using UnityEngine;
using Datatypes.Networking;

namespace Components.Networking
{
    public interface INetworkPositionComponent : IComponent
    {
        SortedList<float, Position> positions { get; }
        Transform transform { get; } 
    }
}
