using System;
using System.Collections;
using UnityEngine;
using Svelto.ECS;
using Services.Networking;
using Nodes.Networking;

namespace Engines.Networking
{
    public class NetworkPositionReplayEngine : INodesEngine, IQueryableNodeEngine, ILateInitEngine
    {
        public IEngineNodeDB nodesDB { set; private get; }

        readonly Type[] _acceptedNodes = { typeof(PlayerNode), typeof(NetworkMobNode) };
        public Type[] AcceptedNodes () { return _acceptedNodes; }

        public void LateInit ()
        {
            TaskRunner.Instance.Run(Heartbeat);
        }

        PlayerNode _currentPlayerNode;
        int _currentBodyID;

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

        IEnumerator Heartbeat ()
        {
            yield return SpectreConnectionConfig.heartbeatEnumerator;
            HeartbeatTick();
        }

        void HeartbeatTick ()
        {
            foreach (NetworkMobNode node in nodesDB.QueryNodes<NetworkMobNode>())
            {
                if (node.ID != _currentBodyID) {

                }
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
