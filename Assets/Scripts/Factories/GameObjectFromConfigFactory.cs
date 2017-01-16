using System;
using Svelto.ES;
using Svelto.Factories;
using System.Collections.Generic;
using UnityEngine;
using MapLoader;
using EntityDescriptors;

namespace Factories {
    public class GameObjectFromConfigFactory : IGameObjectFactory {

        EntitiesData EntitiesData;
        Dictionary<string, GameObject[]> prefabs;

        public GameObjectFromConfigFactory (EntitiesData entitiesData) {
            EntitiesData = entitiesData;
            prefabs = new Dictionary<string, GameObject[]>();
        }

        public GameObject Build (string type) {
            GameObject entity = new GameObject(type);
            entity.AddComponent<DynamicEntityDescriptorHolder>();
            EntityData entityData = EntitiesData.entities[type];
            foreach (KeyValuePair<string, ComponentData> component in entityData.traits) {
                Type implementerFactoryType = Type.GetType("Factories.ImplementerFactories." + component.Key + "ImplementerFactory", true, true);
                entity = (GameObject)implementerFactoryType.GetMethod("BuildAndAttach").Invoke(null, new object[] { entity, component.Value.attributes });
            }
            return entity;
        }

        public IEnumerable<string> EnumerateAllEntities () {
            return EntitiesData.entities.Keys;
        }

        public GameObject Build (GameObject prefab) {
            var copy = UnityEngine.Object.Instantiate(prefab) as GameObject;
            return copy;
        }

        public void RegisterPrefab (GameObject prefab, string type, GameObject parent = null) {
            var objects = new GameObject[2];

            objects[0] = prefab; objects[1] = parent;

            prefabs.Add(type, objects);
        }
    }
}