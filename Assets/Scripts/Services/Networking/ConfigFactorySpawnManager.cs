using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Svelto.ES;
using Svelto.Factories;
using Config;
using Config.Datatypes;

namespace Services.Networking
{
    public static class ConfigFactorySpawnManager
    {
        static IGameObjectFactory _factory;
        static IEntityFactory _entityFactory;
        static WorldConfig _config;
        static Dictionary<NetworkHash128, string> _entityTypeByNetworkHashLookup;

        public static void Initialize (IGameObjectFactory factory, IEntityFactory entityFactory, WorldConfig config)
        {
            _factory = factory;
            _entityFactory = entityFactory;
            _config = config;
            _entityTypeByNetworkHashLookup = new Dictionary<NetworkHash128, string>();

            ClientScene.RegisterPrefab(Resources.Load<GameObject>("Prefabs/PlayerManager"));

            foreach (EntityTypeData entityType in _config.entityTypes.Values)
            {
                ClientScene.RegisterSpawnHandler(entityType.assetId, SpawnEntity, UnspawnEntity);
                _entityTypeByNetworkHashLookup.Add(entityType.assetId, entityType.type);
            }
        }

        static GameObject SpawnEntity (Vector3 position, NetworkHash128 id)
        {
            Debug.Log("Spawning " + id.ToString());
            GameObject go = _factory.Build(_entityTypeByNetworkHashLookup[id]);
            go.transform.position = position;
            _entityFactory.BuildEntity(go.GetInstanceID(), go.GetComponent<IEntityDescriptorHolder>().BuildDescriptorType());
            return go;
        }

        static void UnspawnEntity (GameObject entity)
        {
            Debug.Log("Despawning " + entity.name);
            GameObject.Destroy(entity);
        }
    }
}
