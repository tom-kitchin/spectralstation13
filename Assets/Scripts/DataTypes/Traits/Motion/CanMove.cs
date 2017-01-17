using System;
using UnityEngine;
using Config;

namespace DataTypes.Traits
{
    public class CanMove : Trait
    {
        public float speed;

        public override void BuildAndAttach (ref GameObject go, ref WorldConfig config)
        {
            throw new NotImplementedException("Sorry, no BuildAndAttach for CanMove yet");
        }
    }
}
