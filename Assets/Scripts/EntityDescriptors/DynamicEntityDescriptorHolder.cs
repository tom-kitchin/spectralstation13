using Svelto.ES;
using System;
using Nodes;
using Components;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace EntityDescriptors
{
    /**
     * Use a bunch of reflection to dynamically build an entity descriptor from its collection of components.
     * Expensive, so we should cache the entity descriptor if possible.
     */
    class DynamicEntityDescriptor : EntityDescriptor
    {
        string[] _componentIdentifiers;

        /**
         * Keep a list of all the types under the Nodes namespace, but only build it once.
         */
        static Type[] _nodes;
        public static Type[] Nodes {
            get {
                if (_nodes == null)
                {
                    _nodes = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(NodeWithIDAndRequiredComponents))).ToArray();
                }
                return _nodes;
            }
        }

        public static INodeBuilder[] NodesToBuild (IIdentifiedComponent[] components)
        {
            string[] componentIdentifiers = components.Select(component => component.ComponentIdentifier).ToArray();
            List<INodeBuilder> nodeBuilders = new List<INodeBuilder>();

            // The type of a NodeBuilder with no generic set. We construct the rest with reflection.
            Type emptyNodeBuilderType = typeof(NodeBuilder<>);

            foreach (Type nodeType in Nodes)
            {
                if (NodeRequirementsFulfilled(nodeType, componentIdentifiers))
                {
                    // Use a bunch of reflection to construct a NodeBuilder with the correct node type.
                    Type[] substitutedTypeParameters = { nodeType };
                    Type constructedNodeBuilderType = emptyNodeBuilderType.MakeGenericType(substitutedTypeParameters);
                    var nodeBuilder = (INodeBuilder)Activator.CreateInstance(constructedNodeBuilderType);
                    nodeBuilders.Add(nodeBuilder);
                }
            }

            return nodeBuilders.ToArray();
        }

        private static bool NodeRequirementsFulfilled (Type nodeType, string[] componentIdentifiers)
        {
            string[] nodeRequiredComponents = (string[])nodeType.GetProperty("RequiredComponentIdentifiers").GetValue(null, null);
            // This is a weird one-liner, but it basically checks if nodeRequiredComponents array is a subset of the _componentIdentifiers array.
            return (!nodeRequiredComponents.Except(componentIdentifiers).Any());
        }

        /**
         * Note that we import the _nodesToBuild list from the static call NodesToBuild(), which is the only way I could find to get
         * the superclass to receive the list correctly.
         */
        public DynamicEntityDescriptor (IIdentifiedComponent[] componentsImplementor) : base(NodesToBuild(componentsImplementor), componentsImplementor) { }
    }

    [DisallowMultipleComponent]
    public class DynamicEntityDescriptorHolder : MonoBehaviour, IEntityDescriptorHolder
    {
        EntityDescriptor IEntityDescriptorHolder.BuildDescriptorType ()
        {
            return new DynamicEntityDescriptor(GetComponentsInChildren<IIdentifiedComponent>());
        }
    }
}