using System;
using UnityEngine;
using Svelto.ES;
using Svelto.Ticker;
using Nodes.Motion;

namespace Engines.Motion
{
    public class MovementEngine : INodesEngine, ITickable, IQueryableNodeEngine
    {
        public IEngineNodeDB nodesDB { set; private get; }

        readonly Type[] _acceptedNodes = { typeof(MovementNode) };

        public Type[] AcceptedNodes ()
        {
            return _acceptedNodes;
        }

        public void Add (INode obj)
        {
        }

        public void Remove (INode obj)
        {
        }

        public void Tick (float deltaTime)
        {
            var movementNodeList = nodesDB.QueryNodes<MovementNode>();

            foreach (MovementNode node in movementNodeList)
            {
                if (node.movementComponent.movement.magnitude < 0.1)
                {
                    node.movementComponent.movement = Vector2.zero;
                } else
                {
                    node.movementComponent.transform.Translate(node.movementComponent.movement * deltaTime);
                }
            }
        }
    }
}