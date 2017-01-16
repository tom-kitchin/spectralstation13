using Svelto.ES;
using Nodes;
using Components;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace EntityDescriptors {
    /**
     * Build an EntityDescriptor by just checking every node type to see if it matches. Clunky and has to have every node
     * manually listed, but removes the need to define an entity descriptor for all possible entities.
     */
    class MonolithicEntityDescriptor : EntityDescriptor {
        string[] _componentIdentifiers;

        /**
         * Collect the set of nodes necessary for a given MonolithicEntityDescriptor by manually checking every single node type
         * to see if we have the components it requires. Note we use an extended IComponent, IIdentifiedComponent, which 
         * provides a string name for itself we can compare to the list in NodeWithIDAndRequiredComponents subclasses.
         */
        public static INodeBuilder[] NodesToBuild(IIdentifiedComponent[] components) {
            string[] componentIdentifiers = components.Select(component => component.ComponentIdentifier).ToArray();
            List<INodeBuilder> nodeBuilders = new List<INodeBuilder>();

            // Time for the monolith. All nodes must be checked manually here!
            if (NodeRequirementsFulfilled<Nodes.Test.TestNode>(componentIdentifiers)) { nodeBuilders.Add(new NodeBuilder<Nodes.Test.TestNode>()); }

            return nodeBuilders.ToArray();
        }
        
        private static bool NodeRequirementsFulfilled<TNode> (string[] componentIdentifiers) where TNode : NodeWithIDAndRequiredComponents {
            string[] nodeRequiredComponents = (string[])typeof(TNode).GetProperty("RequiredComponentIdentifiers").GetValue(null, null);
            // This is a weird one-liner, but it basically checks if nodeRequiredComponents array is a subset of the _componentIdentifiers array.
            return (!nodeRequiredComponents.Except(componentIdentifiers).Any());
        }

        /**
         * Override the constructor so that the super sets an empty node list, because we need to work at runtime
         * so we're moving all the work into the BuildNodes override.
         * Some sort of caching would be nice, really.
         */
        public MonolithicEntityDescriptor (IIdentifiedComponent[] componentsImplementor) : base(NodesToBuild(componentsImplementor), componentsImplementor) { }
    }

    [DisallowMultipleComponent]
    public class MonolithicEntityDescriptorHolder : MonoBehaviour, IEntityDescriptorHolder {
        EntityDescriptor IEntityDescriptorHolder.BuildDescriptorType () {
            return new MonolithicEntityDescriptor(GetComponentsInChildren<IIdentifiedComponent>());
        }
    }
}
