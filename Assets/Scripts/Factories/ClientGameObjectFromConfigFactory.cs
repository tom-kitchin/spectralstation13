using UnityEngine;
using UnityEngine.Networking;
using Svelto.Factories;
using Config;
using Datatypes.Config;
using Traits;
using Traits.Networking;

namespace Factories
{
    public class ClientGameObjectFromConfigFactory : GameObjectFromConfigFactory, IGameObjectFactory
    {
        public ClientGameObjectFromConfigFactory (WorldConfig config) : base(config) { }

        public new GameObject Build (string type)
        {
            GameObject entity = new GameObject(type);
            entity.SetActive(false);
            new NetworkMobTrait().BuildAndAttach(ref entity, ref _config);
            EntityTypeData entityType = _config.entityTypes[type];
            foreach (Trait trait in entityType.traits)
            {
                trait.BuildAndAttach(ref entity, ref _config);
            }

            return entity;
        }
    }
}