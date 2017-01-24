using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Config.Datatypes;
using Traits;

namespace Config.Parsers
{
    public class JsonConfigParser : IConfigParser
    {

        public WorldConfig Parse (byte[] entityData)
        {
            WorldConfig worldConfig = new WorldConfig();

            JObject entityJson = LoadJsonFromData(entityData);
            worldConfig.spriteMaps = BuildSpriteMapsFromEntityJson(entityJson);
            worldConfig.entityTypes = BuildEntityTypesFromEntityJson(entityJson);

            return worldConfig;
        }

        public WorldConfig Parse (byte[] entityData, byte[] layoutData)
        {
            WorldConfig worldConfig = Parse(entityData);
            
            JObject layoutJson = LoadJsonFromData(layoutData);
            worldConfig.entities = BuildEntitiesFromLayoutJson(layoutJson);

            return worldConfig;
        }

        SpriteMapDictionary BuildSpriteMapsFromEntityJson (JObject entityObject)
        {
            SpriteMapDictionary spriteMapDictionary = new SpriteMapDictionary();
            foreach (JProperty spriteMapToken in entityObject.GetValue("spriteMaps").Children())
            {
                SpriteMap spriteMap = new SpriteMap();
                spriteMap.name = spriteMapToken.Name;
                spriteMap.cellSize = new Vector2(spriteMapToken.Value["cellSizeX"].Value<int>(), spriteMapToken.Value["cellSizeY"].Value<int>());
                spriteMap.filename = spriteMapToken.Value["file"].Value<string>();
                spriteMapDictionary.Add(spriteMapToken.Name, spriteMap);
            }
            return spriteMapDictionary;
        }

        EntityTypeDataDictionary BuildEntityTypesFromEntityJson (JObject entityObject)
        {
            EntityTypeDataDictionary EntityTypeDataDictionary = new EntityTypeDataDictionary();
            foreach (JProperty entityTypeToken in entityObject.GetValue("entityTypes").Children())
            {
                JToken entityTraitsToken = entityTypeToken.Value["traits"];
                EntityTypeData entityType = new EntityTypeData() { type = entityTypeToken.Name, traits = new List<Trait>() };
                foreach (JProperty traitToken in entityTraitsToken.Children())
                {
                    Type traitDescriptorType = Type.GetType("Traits." + traitToken.Name, true, true);
                    Type traitDescriptorConstructorType = Type.GetType("Config.Parsers.Json.TraitDescriptorBuilders." + traitToken.Name + "Builder", false, true);
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

        EntityDataList BuildEntitiesFromLayoutJson (JObject layoutObject)
        {
            EntityDataList entityCollection = new EntityDataList();

            foreach (JToken entityToken in layoutObject.GetValue("entities").Children())
            {
                entityCollection.Add(entityToken.ToObject<EntityData>());
            }
            return entityCollection;
        }

        JObject LoadJsonFromData (byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (StreamReader sr = new StreamReader(ms))
                {
                    using (JsonTextReader jr = new JsonTextReader(sr))
                    {
                        return JObject.Load(jr);
                    }
                }
            }
        }
    }
}