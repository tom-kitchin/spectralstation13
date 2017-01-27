using UnityEngine;
using Datatypes.Networking;

namespace Components.Networking
{
	public interface INetworkPositionComponent : IComponent
	{
		Position latestPositionBroadcast { set; }
		TruncatableSortedList<double, Position> positions { get; }
		Transform transform { get; }
	}
}
