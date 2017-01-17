using System.Collections.Generic;
using DataTypes.Config;

namespace DataTypes.Traits
{
    public class HasSprites : Trait
    {
        public string startsAs;
        public Dictionary<string, SpriteData> sprites;
    }
}
