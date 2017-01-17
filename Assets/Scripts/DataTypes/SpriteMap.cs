using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
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

    public class SpriteMapDictionary : Dictionary<string, SpriteMap> { }
}
