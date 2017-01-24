using System;
using System.Collections.Generic;

namespace Config.Serializers
{
    [Serializable]
    public class SerializedConfig
    {
        public string mapName;
        public Dictionary<string, byte[]> spriteDatas;
        public byte[] entityData;
    }
}
