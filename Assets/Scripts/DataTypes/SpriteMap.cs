using System.Collections.Generic;
using UnityEngine;

namespace DataTypes
{
    public class SpriteMap
    {
        public Texture2D texture;
        public Vector2 cellSize;
    }

    public class SpriteMapDictionary : Dictionary<string, SpriteMap> { }
}
