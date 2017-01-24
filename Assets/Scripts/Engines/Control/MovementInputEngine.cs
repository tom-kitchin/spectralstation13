using System;
using UnityEngine;
using Svelto.ECS;
using Svelto.Tasks;
using Nodes.Control;
using Nodes.Networking;
using Services.Networking;

namespace Engines.Control
{
    public class MovementInputEngine : INodesEngine, ILateInitEngine, IQueryableNodeEngine
    {
        public IEngineNodeDB nodesDB { set; private get; }

        readonly Type[] _acceptedNodes = { typeof(PlayerNode), typeof(PlayerControlledMovementNode) };

        PlayerNode _currentPlayerNode;
        PlayerControlledMovementNode _currentlyControlledNode;

        public Type[] AcceptedNodes ()
        {
            return _acceptedNodes;
        }

        public void LateInit ()
        {
            TaskRunner.Instance.Run(new TimedLoopActionEnumerator(Tick));
        }

        public void Add (INode obj)
        {
            if (obj is PlayerNode)
            {
                PlayerNode playerNode = obj as PlayerNode;
                if (playerNode.networkEntityComponent.identity.isLocalPlayer)
                {
                    _currentPlayerNode = playerNode;
                }
            }
            if (obj is PlayerControlledMovementNode)
            {
                PlayerControlledMovementNode movementNode = obj as PlayerControlledMovementNode;
                movementNode.isPlayerControllableComponent.currentlyControlled.NotifyOnValueSet(OnCurrentlyControlledChange);
                movementNode.isPlayerControllableComponent.controlledByNodeId.NotifyOnValueSet(OnControlledByChange);

                // DEBUG HACKS
                if (_currentlyControlledNode == null)
                {
                    _currentlyControlledNode = movementNode;
                }
            }
        }

        public void Remove (INode obj)
        {
            if (obj is PlayerNode)
            {
                PlayerNode playerNode = obj as PlayerNode;
                if (playerNode.networkEntityComponent.identity.isLocalPlayer)
                {
                    _currentPlayerNode = null;
                }
            }
            if (obj is PlayerControlledMovementNode)
            {
                PlayerControlledMovementNode movementNode = obj as PlayerControlledMovementNode;
                movementNode.isPlayerControllableComponent.currentlyControlled.StopNotify(OnCurrentlyControlledChange);
                movementNode.isPlayerControllableComponent.controlledByNodeId.StopNotify(OnControlledByChange);
            }
        }

        /**
         * Handle movement inputs.
         */
        void Tick (float deltaTime)
        {
            if (_currentPlayerNode == null) { return; }
            if (_currentlyControlledNode == null) { return; }

            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            Vector2 newMovement = input * _currentlyControlledNode.canMoveComponent.speed;
            if (input != _currentlyControlledNode.movementComponent.movement)
            {
                //_currentlyControlledNode.movementComponent.movement = newMovement;
                _currentlyControlledNode.movementComponent.CmdUpdateMovement(newMovement);
            }
        }

        /**
         * If one of our movement nodes changes control state, 
         * we need to check if it was ours.
         */
        void OnCurrentlyControlledChange (int ID, bool nowControlled)
        {
            if (_currentlyControlledNode != null)
            {
                if (ID == _currentlyControlledNode.ID)
                {
                    if (nowControlled == false)
                    {
                        // Apparently our node thinks it ain't ours (or anyone's) anymore. Harsh.
                        _currentlyControlledNode = null;
                        return;
                    }
                }
            }
        }

        /**
         * If any node changes to a new owner, we need to check
         * if that's now us or used to be us.
         */
        void OnControlledByChange (int ID, int newPlayerNodeId)
        {
            if (_currentlyControlledNode != null)
            {
                if (ID == _currentlyControlledNode.ID)
                {
                    if (newPlayerNodeId != _currentPlayerNode.ID)
                    {
                        // Someone jacked our node! Well, I guess it's theirs now?
                        _currentlyControlledNode = null;
                        return;
                    }
                }
            }
            if (_currentlyControlledNode == null || ID != _currentlyControlledNode.ID)
            {
                if (newPlayerNodeId == _currentPlayerNode.ID)
                {
                    // Woo, new node for us!
                    _currentlyControlledNode = nodesDB.QueryNode<PlayerControlledMovementNode>(ID);
                    return;
                }
            }
        }
    }
}