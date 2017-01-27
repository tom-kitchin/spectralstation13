using System;
using System.Collections.Generic;
using UnityEngine;
using Svelto.ECS;
using Svelto.Tasks;
using Datatypes.Networking;
using Services.Networking;
using Nodes.Networking;

namespace Engines.Client.Networking
{
	/**
	 * Clientside engine which moves all mobile entities except for the one the client controls
	 * according to their movements reported from the server.
	 * Obviously the one the client controls we want to handle differently.
	 * Actually runs on a delay sent from the server, but this engine doesn't need to know that.
	 */
    public class NPCPositionReplayEngine : INodesEngine, IQueryableNodeEngine, ILateInitEngine
    {
        public IEngineNodeDB nodesDB { set; private get; }

		readonly Type[] _acceptedNodes = { typeof(PlayerNode), typeof(NetworkMobNode) };
        public Type[] AcceptedNodes () { return _acceptedNodes; }

		PlayerNode _currentPlayerNode;
		int _currentBodyID;

        public void LateInit ()
        {
			TaskRunner.Instance.Run(new TimedLoopActionEnumerator(Tick));
        }

        public void Add (INode obj)
        {
            if (obj is PlayerNode)
            {
                PlayerNode playerNode = obj as PlayerNode;
                if (playerNode.playerComponent.identity.isLocalPlayer)
                {
                    _currentPlayerNode = playerNode;
                    if (_currentPlayerNode.playerComponent.currentBody != null)
                    {
                        _currentBodyID = playerNode.playerComponent.currentBody.GetInstanceID();
                    }
                    _currentPlayerNode.playerComponent.currentBodyDispatcher.NotifyOnValueSet(CurrentBodyChanged);
                }
            }
        }

        public void Remove (INode obj)
        {
            if (obj is PlayerNode)
            {
                PlayerNode playerNode = obj as PlayerNode;
                if (playerNode.ID == _currentPlayerNode.ID)
                {
                    _currentPlayerNode.playerComponent.currentBodyDispatcher.StopNotify(CurrentBodyChanged);
                    _currentPlayerNode = null;
                }
            }
        }

		/**
		 * Cycle through all mobile entities, skipping the one we control if any, 
		 * and work out where they ought to be based on their provided position list.
		 * Tidy up the list by removing entries which are too old to be useful as we go.
		 * Would this be jerky if we did it on a heartbeat instead of every tick?
		 */
        void Tick (float deltaTime)
        {
			foreach (NetworkMobNode node in nodesDB.QueryNodes<NetworkMobNode>())
			{
				if (_currentPlayerNode != null && _currentBodyID == node.ID) { continue; }

				TruncatableSortedList<double, Position> positions = node.networkPositionComponent.positions;

				// Do nothing if we don't have enough information to work.
				// This should only really be the case for things which haven't moved since they were spawned.
				if (positions.Count < 2)
				{
					continue;
				}

				// Seek through our position list for a position with a timestamp after now.
				// It's a SortedList by timestamp so it's guaranteed to be the right one.
				int nextPositionIndex = -1;
				for (int i = 0; i < node.networkPositionComponent.positions.Count; i++)
				{
					Position position = positions.Values[i];
					if (position.timestamp > SpectreClient.serverTime)
					{
						nextPositionIndex = i;
					}
				}

				// If we have no next position then we're very confused, might as well just remove all entries
				// except the last one since we only need one previous entry.
				if (nextPositionIndex == -1)
				{
					positions.RemoveUntilIndex(positions.Count - 1);
					continue;
				}

				// If our next position is also our first known position, we can't really interpolate, so we'll
				// just have to wait until time ticks past this point. 
				if (nextPositionIndex == 0)
				{
					continue;
				}

				// If our next position is not the second item we're storing old positions we don't need.
				if (nextPositionIndex > 1)
				{
					// Get rid of all except the one position entry before our next one.
					positions.RemoveUntilIndex(nextPositionIndex - 1);
				}

				// Finally we're ready to work. Interpolate the object between the next and previous locations
				// based on timestamps and the current server time.
				Position prevPosition = positions.Values[0];
				Position nextPosition = positions.Values[1];
				double t = (SpectreClient.serverTime - prevPosition.timestamp) / (nextPosition.timestamp - prevPosition.timestamp);
				node.networkPositionComponent.transform.position = Vector2.Lerp(prevPosition.position, nextPosition.position, (float)t);
			}
        }

        void CurrentBodyChanged (int ID, GameObject newBody)
        {
            if (newBody != null)
            {
                _currentBodyID = newBody.GetInstanceID();
            }
        }
    }
}
