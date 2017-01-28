using System;
using System.Collections;
using UnityEngine;
using Svelto.ECS;
using Datatypes.Networking;
using Services.Networking;
using Nodes.Control;
using Nodes.Networking;

namespace Engines.Server.Networking
{
    public class NPCPositionUpdateEngine : INodesEngine, IQueryableNodeEngine, ILateInitEngine
    {
        public IEngineNodeDB nodesDB { set; private get; }

        readonly Type[] _acceptedNodes = { typeof(PlayerControllableNode), typeof(NetworkMobNode) };
        public Type[] AcceptedNodes () { return _acceptedNodes; }

        public void LateInit ()
        {
            TaskRunner.Instance.Run(Heartbeat);
        }

        public void Add (INode obj)
        {
        }

        public void Remove (INode obj)
        {
        }

        IEnumerator Heartbeat ()
        {
            while (true)
            {
                yield return SpectreConnectionConfig.heartbeatEnumerator;
                HeartbeatTick();
            }
        }

        /**
		 * On the heartbeat, go through all mobile entities which are NOT currently player controlled,
		 * and update their reported position for clients taking into account the client delay.
		 * Player controlled entities depend on client input so we don't do that here.
         * If it hasn't moved then we don't need to do anything.
		 */
        void HeartbeatTick ()
        {
            foreach (NetworkMobNode node in nodesDB.QueryNodes<NetworkMobNode>())
            {
                try
                {
                    if (nodesDB.QueryNode<PlayerControllableNode>(node.ID)
                        .isPlayerControllableComponent.controllingPlayer == null)
                    {
                        Debug.Log("Heartbeat from null controllingPlayer");
                        PushDelayedPositionToNetworkPosition(node);
                    }
                } catch (Exception)
                {
                    Debug.Log("Heartbeat from exception");
                    // Frustratingly nodesDB throws standard Exception when it can't find a node. Blame Svelto.
                    PushDelayedPositionToNetworkPosition(node);
                }
            }
        }

        /**
		 * Update reported position. latestPositionBroadcast is sync'd to clients via unity NetworkBehaviour.
		 */
        void PushDelayedPositionToNetworkPosition (NetworkMobNode node)
        {
            Position position = new Position();
            position.position = node.networkPositionComponent.transform.position;
            position.timestamp = Network.time + SpectreConnectionConfig.clientDelay;
            node.networkPositionComponent.latestPositionBroadcast = position;
            Debug.Log("Updated position of " + node.ID + " to " + position.position.ToString() + " with timestamp " + position.timestamp.ToString());
        }
    }
}
