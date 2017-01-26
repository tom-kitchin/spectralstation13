using UnityEngine;
using Svelto.ECS;
using Config;

namespace Traits
{
    public class IsPlayerControllable : Trait
    {
        public override void BuildAndAttach (ref GameObject go, ref WorldConfig config)
        {
            go.AddComponent<Implementers.Control.IsPlayerControllable>();
        }
    }
}
