using System.Collections.Generic;
using DataTypes.Config.Descriptors;

namespace DataTypes.Traits
{
    public class HasSprites : Trait
    {
        public string startsAs;
        public Dictionary<string, SpriteDescriptor> sprites;
    }
}
