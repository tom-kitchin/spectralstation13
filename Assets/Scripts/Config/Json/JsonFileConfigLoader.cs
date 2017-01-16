using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DataTypes.Config;
using DataTypes.Traits;
using DataTypes.Config.Descriptors;

namespace Config.Json
{
    public class JsonFileConfigLoader : IConfigLoader
    {

        string _mapPath;
        WorldConfig _worldConfig;

        public string MapPath { get { return _mapPath; } }
        public WorldConfig WorldConfig {
            get {
                if (_worldConfig == null)
                {
                    Load();
                }
                return _worldConfig;
            }
        }

        public JsonFileConfigLoader (string mapName)
        {
            _mapPath = BuildMapPathFromMapName(mapName);
        }

        WorldConfig Load ()
        {
            if (!Directory.Exists(_mapPath))
            {
                throw new ConfigLoadException("Map directory " + _mapPath + " not found");
            }
            WorldConfig worldConfig = new WorldConfig();

            JObject entityFileJson = LoadJsonFile(Path.Combine(_mapPath, "entityData.json"));
            JObject layoutFileJson = LoadJsonFile(Path.Combine(_mapPath, "layoutData.json"));

            worldConfig.spriteMaps = BuildSpriteMapsFromEntityFileJson(entityFileJson);
            worldConfig.entityTypes = BuildEntityTypesFromEntityFileJson(entityFileJson);
            worldConfig.entities = BuildEntitiesFromLayoutFileJson(layoutFileJson);

            return worldConfig;
        }

        SpriteMapDescriptorCollection BuildSpriteMapsFromEntityFileJson (JObject entityObject)
        {
            SpriteMapDescriptorCollection spriteMapCollection = new SpriteMapDescriptorCollection();
            foreach (JProperty spriteMapToken in entityObject.GetValue("spriteMaps").Children())
            {
                spriteMapCollection.Add(spriteMapToken.Name, spriteMapToken.Value.ToObject<SpriteMapDescriptor>());
            }
            return spriteMapCollection;
        }


        EntityTypeDescriptorCollection BuildEntityTypesFromEntityFileJson (JObject entityObject)
        {
            EntityTypeDescriptorCollection entityTypeCollection = new EntityTypeDescriptorCollection();
            foreach (JProperty entityTypeToken in entityObject.GetValue("entityTypes").Children())
            {
                JToken entityTraitsToken = entityTypeToken.Value["traits"];
                EntityTypeDescriptor entityType = new EntityTypeDescriptor() { traits = new List<Trait>() };
                foreach (JProperty traitToken in entityTraitsToken.Children())
                {
                    Type traitDescriptorType = Type.GetType("DataTypes.Traits." + traitToken.Name, true, true);
                    Type traitDescriptorConstructorType = Type.GetType("Config.Json.TraitDescriptorBuilders." + traitToken.Name + "Builder", false, true);
                    if (traitDescriptorConstructorType != null)
                    {
                        entityType.traits.Add((Trait)traitDescriptorConstructorType.GetMethod("Build").Invoke(null, new object[] { traitToken.Value }));
                    } else
                    {
                        entityType.traits.Add((Trait)traitToken.Value.ToObject(traitDescriptorType));
                    }
                }
                entityTypeCollection.Add(entityTypeToken.Name, entityType);
            }
            return entityTypeCollection;
        }

        EntityDescriptorCollection BuildEntitiesFromLayoutFileJson (JObject layoutObject)
        {
            EntityDescriptorCollection entityCollection = new EntityDescriptorCollection();

            foreach (JToken entityToken in layoutObject.GetValue("entities").Children())
            {
                entityCollection.Add(entityToken.ToObject<EntityDescriptor>());
            }
            return entityCollection;
        }

        JObject LoadJsonFile (string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new ConfigLoadException("Critical config file for map not found at path " + filePath);
            }
            using (StreamReader sr = new StreamReader(filePath))
            {
                using (JsonTextReader jr = new JsonTextReader(sr))
                {
                    return JObject.Load(jr);
                }
            }
        }

        string BuildMapPathFromMapName (string mapName)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string mapsPath = Path.Combine(Path.Combine(documentsPath, "Spectral Station 13"), "Maps");
            string namedMapPath = Path.Combine(mapsPath, mapName);
            return namedMapPath;
        }
    }

    class TraitJsonRepresentationCollection
    {
        List<Trait> traits;
    }

    //class EntityFile
    //{
    //    public Dictionary<string, SpriteMapJsonRepresentation> spriteMaps;
    //    public Dictionary<string, EntityTypeJsonRepresentation> entityTypes;
    //}

    //class LayoutFile
    //{
    //    public List<EntityJsonRepresentation> entities;
    //}

    //class SpriteMapJsonRepresentation
    //{
    //    public string path;
    //    public int cellSizeX;
    //    public int cellSizeY;

    //    public SpriteMapDescriptor ToSpriteMapDescriptor ()
    //    {
    //        return new SpriteMapDescriptor() {
    //            path = path,
    //            cellSize = new Vector2(cellSizeX, cellSizeY)
    //        };
    //    }
    //}

    //class EntityTypeJsonRepresentation
    //{
    //    public Dictionary<string, object> traits;

    //    public EntityTypeDescriptor ToEntityTypeDescriptor ()
    //    {
    //        EntityTypeDescriptor entityType = new EntityTypeDescriptor() {
    //            traits = new List<DataTypes.Traits.Trait>()
    //        };

    //        foreach (KeyValuePair<string, Dictionary<string, object>> traitPair in traits)
    //        {
    //            Type traitDescriptorType = Type.GetType("DataTypes.Traits." + traitPair.Key, true, true);
    //            Trait trait = (Trait)Activator.CreateInstance(traitDescriptorType);
    //            trait.name = traitPair.Key;
    //            trait.unprocessedAttributes = traitPair.Value;
    //            traitDescriptorType.GetMethod("ProcessAttributes").Invoke(trait, new object[] { })

    //            traits.Add((Trait)implementerFactoryType.GetMethod("BuildAndAttach").Invoke(null, new object[] { entity, component.Value.attributes });
    //        }
    //    }
    //}

    //class EntityJsonRepresentation
    //{
    //    public string entityType;
    //    public int cellX;
    //    public int cellY;

    //    public EntityDescriptor ToEntityDescriptor ()
    //    {
    //        return new EntityDescriptor() {
    //            entityType = entityType,
    //            cellCoord = new Vector2(cellX, cellY)
    //        };
    //    }
    //}

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