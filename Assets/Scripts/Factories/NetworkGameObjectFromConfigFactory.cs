using UnityEngine;
using UnityEngine.Networking;
using Svelto.Factories;
using Config;
using Datatypes.Config;
using Traits;
using Traits.Networking;
using EntityDescriptors;

namespace Factories
{
    public class NetworkGameObjectFromConfigFactory : GameObjectFromConfigFactory, IGameObjectFactory
    {
        public NetworkGameObjectFromConfigFactory (WorldConfig config) : base(config) { }

        public new GameObject Build (string type)
        {
            GameObject entity = new GameObject(type);
            new NetworkMobTrait().BuildAndAttach(ref entity, ref _config);
            EntityTypeData entityType = _config.entityTypes[type];
            foreach (Trait trait in entityType.traits)
            {
                trait.BuildAndAttach(ref entity, ref _config);
            }
            // Registering a dynamic GameObject handily allows us to set its assetId
            // so we can catch it clientside with a custom SpawnHandler.
            ClientScene.RegisterPrefab(entity, entityType.assetId);

            return entity;
        }
    }
}