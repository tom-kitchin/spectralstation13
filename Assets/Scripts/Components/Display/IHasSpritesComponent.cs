using System.Collections.Generic;
using UnityEngine;

namespace Components.Display
{
    public interface IHasSpritesComponent : IComponent
    {
        string startsAs { get; }
        Dictionary<string, Sprite> sprites { get; }
    }
}
