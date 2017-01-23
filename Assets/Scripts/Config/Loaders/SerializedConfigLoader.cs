using UnityEngine;
using Config.Parsers;
using Config.Serializers;
using Config.Datatypes;

namespace Config.Loaders
{
    public class SerializedConfigLoader : IConfigLoader
    {
        byte[] _configData;
        IConfigDeserializer _deserializer;

        public SerializedConfigLoader (byte[] configData, IConfigDeserializer deserializer)
        {
            _configData = configData;
            _deserializer = deserializer;
        }

        public WorldConfig Load (IConfigParser parser)
        {
            WorldConfig worldConfig;

            SerializedConfig serializedConfig = _deserializer.Deserialize(_configData);

            worldConfig = parser.Parse(serializedConfig.entityData);
            worldConfig.mapName = serializedConfig.mapName;
            
            foreach (SpriteMap spriteMap in worldConfig.spriteMaps.Values)
            {
                if (serializedConfig.spriteDatas.ContainsKey(spriteMap.filename))
                {
                    spriteMap.texture = LoadTextureFromBytes(serializedConfig.spriteDatas[spriteMap.filename]);
                } else
                {
                    throw new ConfigLoadException("Sprite map not available in serialized config data!");
                }
            }

            return worldConfig;
        }

        Texture2D LoadTextureFromBytes (byte[] data)
        {
            Texture2D texture = new Texture2D(2, 2);
            texture.filterMode = FilterMode.Point;
            texture.LoadImage(data);
            return texture;
        }
    }
}
