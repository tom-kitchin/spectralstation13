using Config.Datatypes;

namespace Config
{
    public class WorldConfig
    {
        public SpriteMapDictionary spriteMaps;
        public EntityTypeDataDictionary entityTypes;
        public EntityDataList entities;

        public static WorldConfig Empty {
            get { return new WorldConfig(); }
        }
    }
}
