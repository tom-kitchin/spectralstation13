using Components;
using Components.Test;
using UnityEngine;

namespace Implementers.Test {
    public class Test : MonoBehaviour, ITestComponent {

        string IIdentifiedComponent.ComponentIdentifier { get { return "test"; } }

        public bool space;
        public string habit;
        public int counter = 0;

        bool ITestComponent.space { get { return space; } }
        string ITestComponent.habit { get { return habit; } }
        int ITestComponent.counter { get { return counter; } set { counter = value; } }
    }
}