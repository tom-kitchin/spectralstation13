using Svelto.ES;
using System;

namespace Nodes {
    public class NodeWithIDAndRequiredComponents : NodeWithID {
        public static string[] RequiredComponentIdentifiers { get { throw new NotImplementedException(); } }
    }
}
