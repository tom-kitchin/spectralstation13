using System.IO;
using System;
using fastJSON;
using System.Collections.Generic;

namespace MapLoader {
    public class JsonFileMapLoader : IMapLoader {

        public MapData LoadMapDataForMap(string mapName) {
            MapData mapData = new MapData();
            string mapDirectory = GetMapDirectoryByName(mapName);
            mapData.EntityConfig = LoadEntityConfigFromJsonFilePath(Path.Combine(mapDirectory, "entityData.json"));
            return mapData;
        }

        private string GetMapDirectoryByName(string mapName) {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string mapsPath = Path.Combine(Path.Combine(documentsPath, "Spectral Station 13"), "Maps");
            if (!Directory.Exists(mapsPath)) {
                Directory.CreateDirectory(mapsPath);
            }
            string namedMapPath = Path.Combine(mapsPath, mapName);
            if (!Directory.Exists(namedMapPath)) {
                throw new MapNotFoundException("Unable to load map " + mapName);
            }
            return namedMapPath;
        }

        private EntitiesData LoadEntityConfigFromJsonFilePath(string filePath) {
            using (StreamReader sr = new StreamReader(filePath)) {
                // EntitiesData entitiesData = JSON.ToObject<EntitiesJsonRepresentation>(sr.ReadToEnd()).ToEntitiesData();
                EntitiesData entitiesData = JSON.ToObject<EntitiesData>(sr.ReadToEnd());
                return entitiesData;
            }
        }
    }

    //class EntitiesJsonRepresentation {
    //    Dictionary<string, Dictionary<string, EntityJsonRepresentation>> categories;

    //    public EntitiesData ToEntitiesData() {
    //        EntitiesData ed = new EntitiesData();
    //        foreach (KeyValuePair<string, Dictionary<string, EntityJsonRepresentation>> categoryPair in categories) {
    //            foreach(KeyValuePair<string, EntityJsonRepresentation> entityPair in categoryPair.Value) {
    //                ed.entities.Add(entityPair.Key, entityPair.Value.ToEntityData());
    //            }
    //        }
    //        return ed;
    //    }
    //}

    //class EntityJsonRepresentation {
    //    public Dictionary<string, ComponentJsonRepresentation> traits;

    //    public EntityData ToEntityData () {
    //        EntityData ed = new EntityData();
    //        ed.traits = new Dictionary<string, ComponentData>();
    //        foreach(KeyValuePair<string, ComponentJsonRepresentation> componentPair in traits) {
    //            ed.traits.Add(componentPair.Key, componentPair.Value.ToComponentData());
    //        }
    //        return ed;
    //    }
    //}

    //class ComponentJsonRepresentation {
    //    public Dictionary<string, object> attributes;

    //    public ComponentData ToComponentData() {
    //        ComponentData cd = new ComponentData();
    //        cd.attributes = attributes;
    //        return cd;
    //    }
    //}
}