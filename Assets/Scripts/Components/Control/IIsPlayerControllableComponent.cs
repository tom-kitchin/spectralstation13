using UnityEngine;

namespace Components.Control
{
    public interface IIsPlayerControllableComponent : IComponent
    {
        GameObject controllingPlayer { get; set; }
    }
}
