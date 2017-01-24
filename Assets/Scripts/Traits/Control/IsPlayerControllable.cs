using UnityEngine;
using Svelto.ECS;
using Config;

namespace Traits
{
    public class IsPlayerControllable : Trait
    {
        public override void BuildAndAttach (ref GameObject go, ref WorldConfig config)
        {
            var isPlayerControllableImplementer = go.AddComponent<Implementers.Control.IsPlayerControllable>();
            isPlayerControllableImplementer.currentlyControlled = new DispatchOnChange<bool>(go.GetInstanceID());
            isPlayerControllableImplementer.controlledByNodeId = new DispatchOnChange<int>(go.GetInstanceID());
        }
    }
}
