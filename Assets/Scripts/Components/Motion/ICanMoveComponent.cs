﻿using Svelto.ES;

namespace Components.Motion
{
    public interface ICanMoveComponent : IComponent
    {
        float speed { get; }
    }
}
