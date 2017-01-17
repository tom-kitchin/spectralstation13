using System;
using System.Collections.Generic;
using UnityEngine;
using Svelto.ES;
using Svelto.Factories;
using DataTypes.Config;
using DataTypes.Traits;
using EntityDescriptors;

namespace Factories {
    public class GameObjectFromConfigFactory : IGameObjectFactory {

        EntityTypeDataDictionary _entityTypes;
        Dictionary<string, GameObject[]> _prefabs;

        public GameObjectFromConfigFactory (EntityTypeDataDictionary entityTypes) {
            _entityTypes = entityTypes;
            _prefabs = new Dictionary<string, GameObject[]>();
        }

        public GameObject Build (string type) {
            GameObject entity = new GameObject(type);
            entity.AddComponent<DynamicEntityDescriptorHolder>();
            EntityTypeData entityType = _entityTypes[type];
            foreach (Trait trait in entityType.traits) {
                Debug.Log(trait);
                //Type implementerFactoryType = Type.GetType("Factories.ImplementerFactories." + component.Key + "ImplementerFactory", true, true);
                //entity = (GameObject)implementerFactoryType.GetMethod("BuildAndAttach").Invoke(null, new object[] { entity, component.Value.attributes });
            }
            return entity;
        }

        public GameObject Build (GameObject prefab) {
            var copy = UnityEngine.Object.Instantiate(prefab) as GameObject;
            return copy;
        }

        public void RegisterPrefab (GameObject prefab, string type, GameObject parent = null) {
            var objects = new GameObject[2];

            objects[0] = prefab; objects[1] = parent;

            _prefabs.Add(type, objects);
        }
    }
}