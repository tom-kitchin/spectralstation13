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
            }
        }

        /**
         * Handle movement inputs.
         */
        void Tick (float deltaTime)
        {
            // If we actually have a current player (so we're a client or a host) then movement inputs should be both stored
            // and sent to the server.
            if (_currentPlayerNode != null)
            {
                Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                if (input != _currentPlayerNode.movementControlComponent.movementInput)
                {
                    _currentPlayerNode.movementControlComponent.CmdSetMovementInput(input);
                    _currentPlayerNode.movementControlComponent.movementInput = input;
                }
            }

            // Read out stored inputs and apply them for each Player and the Body they currently control, if relevant.
            foreach (PlayerMovementControlNode playerNode in nodesDB.QueryNodes<PlayerMovementControlNode>())
            {
                PlayerControlledMovementNode bodyNode = GetActiveBodyNodeForPlayer(playerNode);
                if (bodyNode != null)
                {
                    bodyNode.movementComponent.movement = playerNode.movementControlComponent.movementInput.normalized * bodyNode.canMoveComponent.speed;
                }
            }
        }

        /**
         * If the player's body is active, properly set up and so on, get the control node representing the thing they're controlling (the "body"). Otherwise null.
         */
        PlayerControlledMovementNode GetActiveBodyNodeForPlayer (PlayerMovementControlNode playerNode)
        {
            if (playerNode != null && playerNode.movementControlComponent.listening && playerNode.playerComponent.currentBody != null)
            {
                return nodesDB.QueryNode<PlayerControlledMovementNode>(playerNode.playerComponent.currentBody.GetInstanceID());
            }
            return null;
        }
    }
}