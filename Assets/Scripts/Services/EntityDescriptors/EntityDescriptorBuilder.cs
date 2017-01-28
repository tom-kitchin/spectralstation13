using UnityEngine;
using Svelto.ECS;
using Components;
using EntityDescriptors;

namespace Services.EntityDescriptors
{
    /**
     * Bespoke EntityDescriptors, generated to your requirements!
     * We just return DynamicEntityDescriptors at the moment, but in theory
     * we could properly get the right EntityDescriptor if there's a defined one.
     */
    public static class EntityDescriptorBuilder
    {
        public static EntityDescriptor BuildEntityDescriptor(GameObject go)
        {
            return new DynamicEntityDescriptor(go.GetComponents<IComponent>());
        }
    }
}
