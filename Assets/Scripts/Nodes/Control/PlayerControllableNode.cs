using Svelto.ECS;
using Components.Control;
using Components.Networking;

namespace Nodes.Control
{
	public class PlayerControllableNode : NodeWithID
	{
		public IIsPlayerControllableComponent isPlayerControllableComponent;
	}
}
