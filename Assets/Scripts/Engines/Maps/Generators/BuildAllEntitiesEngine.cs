using Svelto.ES;
using Factories;
using UnityEngine;

namespace Engines.Maps.Generators {
    public class BuildAllEntitiesEngine : IEngine {

        public BuildAllEntitiesEngine(GameObjectFromConfigFactory factory, IEntityFactory entityFactory, Dispatcher<int> contextInitializeEvent) {
            _factory = factory;
            _entityFactory = entityFactory;
            contextInitializeEvent.subscribers += BuildAllEntities;
        }

        void BuildAllEntities(int ID) {
            foreach(string name in _factory.EnumerateAllEntities()) {
                GameObject go = _factory.Build(name);
                _entityFactory.BuildEntity(go.GetInstanceID(), go.GetComponent<IEntityDescriptorHolder>().BuildDescriptorType());
            }
        }

        GameObjectFromConfigFactory _factory;
        IEntityFactory _entityFactory;
    }
}
