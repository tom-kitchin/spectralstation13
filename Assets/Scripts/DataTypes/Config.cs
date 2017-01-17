using System.Collections.Generic;
using UnityEngine;
using DataTypes.Traits;

namespace DataTypes.Config
{
    public class EntityTypeData
    {
        public List<Trait> traits;
    }

    public class EntityData
    {
        public string entityType;
        public Vector2 cellCoord;
        public Vector2 subCellCoord;
        
        public int cellX { set { cellCoord.x = value; } }
        public int cellY { set { cellCoord.y = value; } }
        public int subCellX { set { subCellCoord.x = value; } }
        public int subCellY { set { subCellCoord.y = value; } }
    }

    public class SpriteData
    {
        public string spriteMap;
        public Vector2 cellCoord;

        public int cellX { set { cellCoord.x = value; } }
        public int cellY { set { cellCoord.y = value; } }
    }

    public class EntityDataList : List<EntityData> { }
    public class EntityTypeDataDictionary : Dictionary<string, EntityTypeData> { }
}