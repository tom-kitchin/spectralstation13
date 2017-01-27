using System.Collections.Generic;
using UnityEngine;
using Datatypes.Networking;

namespace Components.Networking
{
	public interface INetworkPositionComponent : IComponent
	{
		Position latestPositionBroadcast { set; }
		SortedList<double, Position> positions { get; }
		Transform transform { get; }
	}
}
