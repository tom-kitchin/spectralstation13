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

                double t = SpectreClient.serverTime;

                TimestampedList<Position> positions = node.networkPositionComponent.positions;

                // Do nothing if we don't have enough information to work with.
                // This should only really be the case for things which haven't moved since they were spawned.
                if (positions.Count < 2)
                {
                    continue;
                }

                // Clean up the list, throwing away entries earlier than the one immediately before now.
                positions.RemoveOutdatedTimestamps(t);

                // Seek through our position list for a position with a timestamp after now.
                int nextPositionIndex = positions.GetIndexAfterTimestamp(t);

                // If our next position is not the second entry then after cleanup we don't have enough
                // to work with - either we have no previous entry or no next entry.
                if (nextPositionIndex != 1)
                {
                    continue;
                }

                // Finally we're ready to work. Interpolate the object between the next and previous locations
                // based on timestamps and the current server time.
                Position prevPosition = positions.GetByIndex(0);
                Position nextPosition = positions.GetByIndex(1);
                double normalizedT = (t - prevPosition.timestamp) / (nextPosition.timestamp - prevPosition.timestamp);
                node.networkPositionComponent.transform.position = Vector2.Lerp(prevPosition.position, nextPosition.position, (float)normalizedT);
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
