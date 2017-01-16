using UnityEngine;
using System.Collections.Generic;

namespace Factories.ImplementerFactories {
    static class TestImplementerFactory {
        public static GameObject BuildAndAttach (GameObject go, Dictionary<string, object> attributes) {
            Implementers.Test.Test implementer = go.AddComponent<Implementers.Test.Test>();
            implementer.space = (bool)attributes["space"];
            implementer.habit = (string)attributes["habit"];
            return go;
        }
    }
}
