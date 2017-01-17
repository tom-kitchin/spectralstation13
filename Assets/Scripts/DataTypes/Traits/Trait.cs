using UnityEngine;
using Config;

namespace DataTypes.Traits
{
    public abstract class Trait
    {
        public abstract void BuildAndAttach (ref GameObject go, ref WorldConfig config);
    }
}
