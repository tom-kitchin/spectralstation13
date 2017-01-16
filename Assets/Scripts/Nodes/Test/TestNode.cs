using Components.Test;

namespace Nodes.Test {

    public class TestNode : NodeWithIDAndRequiredComponents {
        public ITestComponent testComponent;

        new public static string[] RequiredComponentIdentifiers {
            get {
                return new string[] { "test" };
            }
        }
    }
}
