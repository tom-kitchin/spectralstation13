using System;
using System.Collections.Generic;

namespace MapLoader {
    public class MapData {
        public EntitiesData EntityConfig;
    }

    public class EntitiesData {
        public Dictionary<string, EntityData> entities;
    }

    public class EntityData {
        public Dictionary<string, ComponentData> traits;
    }

    public class ComponentData {
        public Dictionary<string, object> attributes;
    }
    
    public class MapNotFoundException : Exception {
        public MapNotFoundException () { }

        public MapNotFoundException (string message) : base(message) { }

        public MapNotFoundException (string message, Exception inner) : base(message, inner) { }
    }
}