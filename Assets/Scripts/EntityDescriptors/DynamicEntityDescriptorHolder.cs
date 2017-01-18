using Svelto.ES;
using System;
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
        /**
         * Keep a list of all the Component types used by all the Nodes, stored by Node, but only build it once (as it shouldn't change).
         */
        static Dictionary<Type, Type[]> _nodeComponents;
        public static Dictionary<Type, Type[]> NodeComponents {
            get {
                if (_nodeComponents == null)
                {
                    _nodeComponents = new Dictionary<Type, Type[]>();
                    foreach (Type nodeType in Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsSubclassOf(typeof(NodeWithID))))
                    {
                        _nodeComponents.Add(nodeType, nodeType.GetFields().Select(t => t.FieldType).ToArray());
                    }
                }
                return _nodeComponents;
            }
        }

        public static INodeBuilder[] NodesToBuild (IComponent[] implementers)
        {
            // The type of a NodeBuilder with no generic set. We use this to build typed NodeBuilders via reflection on demand.
            Type emptyNodeBuilderType = typeof(NodeBuilder<>);

            List<INodeBuilder> nodeBuilders = new List<INodeBuilder>();
            Type[] componentTypes = GetImplementedComponentsListFromImplementers(implementers);
            foreach (KeyValuePair<Type, Type[]> nodeRequirementsPair in NodeComponents)
            {
                if (NodeRequirementsFulfilled(nodeRequirementsPair.Value, componentTypes))
                {
                    // Use a bunch of reflection to construct a NodeBuilder with the correct node type.
                    Type[] substitutedTypeParameters = { nodeRequirementsPair.Key };
                    Type constructedNodeBuilderType = emptyNodeBuilderType.MakeGenericType(substitutedTypeParameters);
                    var nodeBuilder = (INodeBuilder)Activator.CreateInstance(constructedNodeBuilderType);
                    nodeBuilders.Add(nodeBuilder);
                }
            }

            return nodeBuilders.ToArray();
        }

        private static Type[] GetImplementedComponentsListFromImplementers (IComponent[] implementers)
        {
            HashSet<Type> interfaces = new HashSet<Type>();
            foreach (IComponent implementer in implementers)
            {
                foreach (Type interfaceType in implementer.GetType().GetInterfaces())
                {
                    if (typeof(IComponent).IsAssignableFrom(interfaceType))
                    {
                        interfaces.Add(interfaceType);
                    }
                }
            }
            interfaces.Remove(typeof(IComponent));
            return interfaces.ToArray();
        }

        private static bool NodeRequirementsFulfilled (Type[] nodeRequirements, Type[] components)
        {
            // This is a weird one-liner, but it basically checks if nodeRequiredComponents array is a subset of the _componentIdentifiers array.
            return (!nodeRequirements.Except(components).Any());
        }

        /**
         * Note that we import the _nodesToBuild list from the static call NodesToBuild(), which is the only way I could find to get
         * the superclass to receive the list correctly.
         */
        public DynamicEntityDescriptor (IComponent[] componentsImplementor) : base(NodesToBuild(componentsImplementor), componentsImplementor) { }
    }

    [DisallowMultipleComponent]
    public class DynamicEntityDescriptorHolder : MonoBehaviour, IEntityDescriptorHolder
    {
        EntityDescriptor IEntityDescriptorHolder.BuildDescriptorType ()
        {
            return new DynamicEntityDescriptor(GetComponentsInChildren<IComponent>());
        }
    }
}