using UnityEngine;
using Components.Display;
using System.Collections.Generic;

namespace Implementers.Display
{
    public class HasSprites : MonoBehaviour, IHasSpritesComponent
    {
        public Dictionary<string, Sprite> sprites;
        public string startsAs;

        Dictionary<string, Sprite> IHasSpritesComponent.sprites { get { return sprites; } }
        string IHasSpritesComponent.startsAs { get { return startsAs; } }
    }
}
