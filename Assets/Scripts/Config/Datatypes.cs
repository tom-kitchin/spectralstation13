using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Traits;

namespace Config.Datatypes
{
    public class EntityTypeData : INetworkSpawnable
    {
        public string type;
        public List<Trait> traits;
        public NetworkHash128 assetId {
            get {
                if (!_hasGeneratedAssetId)
                {
                    if (type == null)
                    {
                        throw new FormatException("EntityTypeData has no type name, so can't generate an assetId");
                    }
                    _assetId = AssetIdHelper.GenerateAssetIdFromString(type);
                    _hasGeneratedAssetId = true;
                } 
                return _assetId;
            }
        }

        NetworkHash128 _assetId;
        bool _hasGeneratedAssetId = false;
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
        public string name;
        public Texture2D texture;
        public Vector2 cellSize;
        public string filename;

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