using UnityEngine;
using UnityEngine.Networking;
using Svelto.ES;
using Svelto.Factories;
using Config;

namespace Factories
{
    public class ConfigFactorySpawnManager
    {
        IGameObjectFactory _factory;
        IEntityFactory _entityFactory;
        WorldConfig _config;

        public ConfigFactorySpawnManager (IGameObjectFactory factory, IEntityFactory entityFactory, WorldConfig config)
        {
            _factory = factory;
            _entityFactory = entityFactory;
            _config = config;

            foreach (string entityTypeName in config.entityTypes.Keys)
            {
                ClientScene.RegisterSpawnHandler(NetworkHash128.Parse(entityTypeName), SpawnEntity, UnspawnEntity);
            }
        }

        GameObject SpawnEntity (Vector3 position, NetworkHash128 id)
        {
            Debug.Log("Spawning " + id.ToString());
            GameObject go = _factory.Build(id.ToString());
            go.transform.position = position;
            _entityFactory.BuildEntity(go.GetInstanceID(), go.GetComponent<IEntityDescriptorHolder>().BuildDescriptorType());
            return go;
        }

        void UnspawnEntity (GameObject entity)
        {
            Debug.Log("Despawning " + entity.name);
            GameObject.Destroy(entity);
        }
    }
}
