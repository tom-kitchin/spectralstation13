using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    public class SpriteMap
    {
        public Texture2D spriteMap;
        public Vector2 cellSize;
    }

    public class SpriteMapCollection : Dictionary<string, SpriteMap> { }
}
