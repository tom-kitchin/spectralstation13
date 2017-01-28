using System;
using System.Collections;
using UnityEngine;
using Svelto.ECS;
using Svelto.Factories;
using Services.Networking;
using Services.EntityDescriptors;
using Nodes.Control;
using Nodes.Networking;

namespace Engines.Server.Control
{
    /**
     * Engine which just gives new players a body to drive around.
     */
    public class PlayerBodyCreationEngine : INodesEngine, IQueryableNodeEngine
    {
        public IEngineNodeDB nodesDB { set; private get; }

        readonly Type[] _acceptedNodes = { typeof(PlayerNode), typeof(PlayerControllableNode) };
        public Type[] AcceptedNodes () { return _acceptedNodes; }

        IGameObjectFactory _factory;
        IEntityFactory _entityFactory;

        public PlayerBodyCreationEngine (IGameObjectFactory factory, IEntityFactory entityFactory)
        {
            _factory = factory;
            _entityFactory = entityFactory;
        }

        public void Add (INode obj)
        {
            if (obj is PlayerNode)
            {
                // Build the player a body. In the end we should handle this differently, plus we need to know
                // if the given body is player controllable, etc. etc.
                PlayerNode playerNode = obj as PlayerNode;
                GameObject body = _factory.Build("robot");
                body.transform.position = Vector2.zero;
                _entityFactory.BuildEntity(body.GetInstanceID(), EntityDescriptorBuilder.BuildEntityDescriptor(body));
                SpectreServer.Spawn(body);
                TaskRunner.Instance.Run(SetBodyOwner(playerNode, body));
            }
        }

        public void Remove (INode obj)
        {
            if (obj is PlayerNode)
            {
                PlayerNode playerNode = obj as PlayerNode;
                PlayerControllableNode bodyNode = nodesDB.QueryNode<PlayerControllableNode>(playerNode.playerComponent.currentBody.GetInstanceID());
                bodyNode.isPlayerControllableComponent.controllingPlayer = null;
            }
        }

        IEnumerator SetBodyOwner (PlayerNode owner, GameObject body)
        {
            PlayerControllableNode bodyNode = null;
            while (bodyNode == null) {
                try
                {
                    bodyNode = nodesDB.QueryNode<PlayerControllableNode>(body.GetInstanceID());
                } catch (Exception)
                {
                }
                yield return null;
            }
            Debug.Log("Found node for new body, id: " + bodyNode.ID);
            owner.playerComponent.currentBody = body;
            bodyNode.isPlayerControllableComponent.controllingPlayer = owner.playerComponent.manager;
        }
    }
}
