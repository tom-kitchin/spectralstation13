using Config.Datatypes;

namespace Config
{
    public class WorldConfig
    {
        public string mapName;
        public SpriteMapDictionary spriteMaps;
        public EntityTypeDataDictionary entityTypes;
        public EntityDataList entities;

        public WorldConfig (string name)
        {
            mapName = name;
        }

        public static WorldConfig Empty {
            get { return new WorldConfig(null); }
        }
    }
}
