using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Config.Datatypes;
using Traits;

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

        void Load ()
        {
            if (!Directory.Exists(_mapPath))
            {
                throw new ConfigLoadException("Map directory " + _mapPath + " not found");
            }
            _worldConfig = new WorldConfig();

            JObject entityFileJson = LoadJsonFile(Path.Combine(_mapPath, "entityData.json"));
            JObject layoutFileJson = LoadJsonFile(Path.Combine(_mapPath, "layoutData.json"));

            _worldConfig.spriteMaps = BuildSpriteMapsFromEntityFileJson(entityFileJson);
            _worldConfig.entityTypes = BuildEntityTypesFromEntityFileJson(entityFileJson);
            _worldConfig.entities = BuildEntitiesFromLayoutFileJson(layoutFileJson);
        }

        SpriteMapDictionary BuildSpriteMapsFromEntityFileJson (JObject entityObject)
        {
            SpriteMapDictionary spriteMapDictionary = new SpriteMapDictionary();
            foreach (JProperty spriteMapToken in entityObject.GetValue("spriteMaps").Children())
            {
                SpriteMap spriteMap = new SpriteMap();
                string texturePath = Path.Combine(_mapPath, spriteMapToken.Value["path"].Value<string>());
                spriteMap.texture = LoadTextureFromPath(texturePath);
                spriteMap.cellSize = new Vector2(spriteMapToken.Value["cellSizeX"].Value<int>(), spriteMapToken.Value["cellSizeY"].Value<int>());
                spriteMapDictionary.Add(spriteMapToken.Name, spriteMap);
            }
            return spriteMapDictionary;
        }

        Texture2D LoadTextureFromPath (string path)
        {
            byte[] fileData = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.filterMode = FilterMode.Point;
            texture.LoadImage(fileData);
            return texture;
        }

        EntityTypeDataDictionary BuildEntityTypesFromEntityFileJson (JObject entityObject)
        {
            EntityTypeDataDictionary EntityTypeDataDictionary = new EntityTypeDataDictionary();
            foreach (JProperty entityTypeToken in entityObject.GetValue("entityTypes").Children())
            {
                JToken entityTraitsToken = entityTypeToken.Value["traits"];
                EntityTypeData entityType = new EntityTypeData() { traits = new List<Trait>() };
                foreach (JProperty traitToken in entityTraitsToken.Children())
                {
                    Type traitDescriptorType = Type.GetType("Traits." + traitToken.Name, true, true);
                    Type traitDescriptorConstructorType = Type.GetType("Config.Json.TraitDescriptorBuilders." + traitToken.Name + "Builder", false, true);
                    if (traitDescriptorConstructorType != null)
                    {
                        entityType.traits.Add((Trait)traitDescriptorConstructorType.GetMethod("Build").Invoke(null, new object[] { traitToken.Value }));
                    } else
                    {
                        entityType.traits.Add((Trait)traitToken.Value.ToObject(traitDescriptorType));
                    }
                }
                EntityTypeDataDictionary.Add(entityTypeToken.Name, entityType);
            }
            return EntityTypeDataDictionary;
        }

        EntityDataList BuildEntitiesFromLayoutFileJson (JObject layoutObject)
        {
            EntityDataList entityCollection = new EntityDataList();

            foreach (JToken entityToken in layoutObject.GetValue("entities").Children())
            {
                entityCollection.Add(entityToken.ToObject<EntityData>());
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
}