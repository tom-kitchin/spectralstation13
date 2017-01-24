using UnityEngine;
using Svelto.ES;
using Components.Control;
using Implementers.Networking;

namespace Implementers.Control
{
    public class IsPlayerControllable : MonoBehaviour, IIsPlayerControllableComponent
    {
        public PlayerManager controlledBy { get { _controlledBy._value; } set { _controlledBy.value = value; } }

        DispatcherOnChange<int, PlayerManager> _controlledBy;

        DispatcherOnChange<int, PlayerManager> IIsPlayerControllableComponent.controlledBy { get { return _controlledBy; } set { _controlledBy = value; } }
    }
}
