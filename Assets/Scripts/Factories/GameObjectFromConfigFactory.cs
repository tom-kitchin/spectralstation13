using System.Collections.Generic;
using UnityEngine;
using Svelto.Factories;
using Config;
using Config.Datatypes;
using Traits;
using EntityDescriptors;

namespace Factories
{
    public class GameObjectFromConfigFactory : IGameObjectFactory
    {
        protected WorldConfig _config;
        protected Dictionary<string, GameObject[]> _prefabs;

        public GameObjectFromConfigFactory (WorldConfig config)
        {
            _config = config;
            _prefabs = new Dictionary<string, GameObject[]>();
        }

        public GameObject Build (string type)
        {
            GameObject entity = new GameObject(type);
            entity.AddComponent<DynamicEntityDescriptorHolder>();
            EntityTypeData entityType = _config.entityTypes[type];
            foreach (Trait trait in entityType.traits)
            {
                trait.BuildAndAttach(ref entity, ref _config);
            }
            return entity;
        }

        public GameObject Build (GameObject prefab)
        {
            var copy = UnityEngine.Object.Instantiate(prefab) as GameObject;
            return copy;
        }

        public void RegisterPrefab (GameObject prefab, string type, GameObject parent = null)
        {
            var objects = new GameObject[2];

            objects[0] = prefab; objects[1] = parent;

            _prefabs.Add(type, objects);
        }
    }
}