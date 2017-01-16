using Svelto.ES;
using Nodes.Test;
using UnityEngine;

namespace EntityDescriptors.Test {
    class TestEntityDescriptor : EntityDescriptor {
        static readonly INodeBuilder[] _nodesToBuild;

        static TestEntityDescriptor() {
            _nodesToBuild = new INodeBuilder[] {
                new NodeBuilder<TestNode>()
            };
        }

        public TestEntityDescriptor (IComponent[] componentsImplementor) : base(_nodesToBuild, componentsImplementor) { }
    }

    [DisallowMultipleComponent]
    public class TestEntityDescriptorHolder : MonoBehaviour, IEntityDescriptorHolder {
        EntityDescriptor IEntityDescriptorHolder.BuildDescriptorType() {
            return new TestEntityDescriptor(GetComponentsInChildren<IComponent>());
        }
    }
}
