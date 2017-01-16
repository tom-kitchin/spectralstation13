using System.Collections.Generic;
using UnityEngine;
using DataTypes.Traits;

namespace DataTypes.Config.Descriptors
{
    public class EntityTypeDescriptor
    {
        public List<Trait> traits;
    }

    public class EntityDescriptor
    {
        public string entityType;
        public Vector2 cellCoord;
        public Vector2 subCellCoord;
        
        public int cellX { set { cellCoord.x = value; } }
        public int cellY { set { cellCoord.y = value; } }
        public int subCellX { set { subCellCoord.x = value; } }
        public int subCellY { set { subCellCoord.y = value; } }
    }

    public class SpriteMapDescriptor
    {
        public string path;
        public Vector2 cellSize;

        public int cellSizeX { set { cellSize.x = value; } }
        public int cellSizeY { set { cellSize.y = value; } }
    }

    public class SpriteDescriptor
    {
        public string spriteMap;
        public Vector2 cellCoord;

        public int cellX { set { cellCoord.x = value; } }
        public int cellY { set { cellCoord.y = value; } }
    }

    public class EntityDescriptorCollection : List<EntityDescriptor> { }
    public class EntityTypeDescriptorCollection : Dictionary<string, EntityTypeDescriptor> { }
    public class SpriteMapDescriptorCollection : Dictionary<string, SpriteMapDescriptor> { }
}