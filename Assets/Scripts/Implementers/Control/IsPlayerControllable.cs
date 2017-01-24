using UnityEngine;
using Svelto.ECS;
using Components.Control;
using Implementers.Networking;

namespace Implementers.Control
{
    public class IsPlayerControllable : MonoBehaviour, IIsPlayerControllableComponent
    {
        public PlayerManager controlledBy {
            get {
                return _controlledBy;
            }
            set {
                _controlledBy = value;
                if (value == null)
                {
                    currentlyControlled.value = false;
                } else
                {
                    currentlyControlled.value = true;
                    controlledByNodeId.value = value.GetInstanceID();
                }
            }
        }
        PlayerManager _controlledBy;

        public DispatchOnChange<bool> currentlyControlled;
        public DispatchOnChange<int> controlledByNodeId;

        DispatchOnChange<bool> IIsPlayerControllableComponent.currentlyControlled {  get { return currentlyControlled; } }
        DispatchOnChange<int> IIsPlayerControllableComponent.controlledByNodeId { get { return controlledByNodeId; } }
    }
}
