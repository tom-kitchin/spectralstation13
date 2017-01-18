using System.Collections.Generic;
using UnityEngine;
using Traits;

namespace Config.Datatypes
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

    public class SpriteMap
    {
        public Texture2D texture;
        public Vector2 cellSize;

        public Vector2 CellOrigin (Vector2 cellCoord)
        {
            return new Vector2(cellCoord.x * cellSize.x, cellCoord.y * cellSize.y);
        }

        public Rect CellRectangle (Vector2 cellCoord)
        {
            return new Rect(CellOrigin(cellCoord), cellSize);
        }
    }

    public class EntityDataList : List<EntityData> { }
    public class EntityTypeDataDictionary : Dictionary<string, EntityTypeData> { }
    public class SpriteMapDictionary : Dictionary<string, SpriteMap> { }
}