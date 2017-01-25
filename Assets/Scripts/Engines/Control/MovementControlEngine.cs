using System;
using UnityEngine;
using Svelto.ECS;
using Svelto.Tasks;
using Nodes.Control;

namespace Engines.Control
{
    public class MovementControlEngine : INodesEngine, ILateInitEngine, IQueryableNodeEngine
    {
        public IEngineNodeDB nodesDB { set; private get; }

        readonly Type[] _acceptedNodes = {
            typeof(PlayerMovementControlNode),
            typeof(PlayerControlledMovementNode)
        };

        PlayerMovementControlNode _currentPlayerNode;

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
            if (obj is PlayerMovementControlNode)
            {
                PlayerMovementControlNode playerNode = obj as PlayerMovementControlNode;
                if (playerNode.playerComponent.identity.isLocalPlayer)
                {
                    _currentPlayerNode = playerNode;
                }
                playerNode.movementControlComponent.movementInput.NotifyOnValueSet(MovementInputChange);
            }
        }

        public void Remove (INode obj)
        {
            if (obj is PlayerMovementControlNode)
            {
                PlayerMovementControlNode playerNode = obj as PlayerMovementControlNode;
                if (playerNode.ID == _currentPlayerNode.ID)
                {
                    _currentPlayerNode = null;
                }
                playerNode.movementControlComponent.movementInput.StopNotify(MovementInputChange);
            }
        }

        /**
         * Handle movement inputs.
         */
        void Tick (float deltaTime)
        {
            if (_currentPlayerNode == null) { return; }

            Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (input != _currentPlayerNode.movementControlComponent.movementInput.value)
            {
                _currentPlayerNode.movementControlComponent.CmdSetMovementInput(input);
                _currentPlayerNode.movementControlComponent.movementInput.value = input;
            }
        }

        /**
         * When the movement input for a player control changes, check it against the target speed and then apply it.
         */
        void MovementInputChange (int ID, Vector2 newMovement)
        {
            PlayerControlledMovementNode controlTarget = GetActiveBodyNodeForPlayer(nodesDB.QueryNode<PlayerMovementControlNode>(ID));
            if (controlTarget == null) { return; }

            Debug.Log("Movement input change detected! Changed to: " + newMovement.ToString());
            
            controlTarget.movementComponent.movement = newMovement.normalized * controlTarget.canMoveComponent.speed;
        }

        /**
         * If the player's body is active, properly set up and so on, get the control node representing the thing they're controlling (the "body"). Otherwise null.
         */
        PlayerControlledMovementNode GetActiveBodyNodeForPlayer (PlayerMovementControlNode playerNode)
        {
            if (playerNode != null && playerNode.movementControlComponent.active && playerNode.playerComponent.currentBody != null)
            {
                return nodesDB.QueryNode<PlayerControlledMovementNode>(playerNode.playerComponent.currentBody.GetInstanceID());
            }
            return null;
        }
    }
}