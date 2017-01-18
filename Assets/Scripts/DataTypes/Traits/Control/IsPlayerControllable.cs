using UnityEngine;
using Config;

namespace DataTypes.Traits
{
    public class IsPlayerControllable : Trait
    {
        public override void BuildAndAttach (ref GameObject go, ref WorldConfig config)
        {
            var isPlayerControllableImplementer = go.AddComponent<Implementers.Control.IsPlayerControllable>();
            isPlayerControllableImplementer.currentlyControlled = false;
        }
    }
}
