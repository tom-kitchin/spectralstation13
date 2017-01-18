using UnityEngine;
using Config;

namespace Traits
{
    public class CanMove : Trait
    {
        public float speed;

        public override void BuildAndAttach (ref GameObject go, ref WorldConfig config)
        {
            var canMoveImplementer = go.AddComponent<Implementers.Motion.CanMove>();
            canMoveImplementer.speed = speed;
            canMoveImplementer._transform = go.transform;
        }
    }
}
