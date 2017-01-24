﻿using UnityEngine;
using UnityEngine.Networking;
using Svelto.Factories;
using Config;
using Config.Datatypes;
using Traits;
using Traits.Networking;
using EntityDescriptors;

namespace Factories
{
    public class ClientGameObjectFromConfigFactory : GameObjectFromConfigFactory, IGameObjectFactory
    {
        public ClientGameObjectFromConfigFactory (WorldConfig config) : base(config) { }

        public new GameObject Build (string type)
        {
            GameObject entity = new GameObject(type);
            entity.SetActive(false);
            entity.AddComponent<DynamicEntityDescriptorHolder>();
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