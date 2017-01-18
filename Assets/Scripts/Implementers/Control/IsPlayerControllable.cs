using UnityEngine;
using Components.Control;

namespace Implementers.Control
{
    public class IsPlayerControllable : MonoBehaviour, IIsPlayerControllableComponent
    {
        public bool currentlyControlled;

        bool IIsPlayerControllableComponent.currentlyControlled { get { return currentlyControlled; } set { currentlyControlled = value; } }
    }
}
