using Components.Display;

namespace Nodes.Display
{
    public class HasSpritesNode : NodeWithIDAndRequiredComponents
    {
        public IHasSpritesComponent hasSpritesComponent;

        new public static string[] RequiredComponentIdentifiers {
            get {
                return new string[] { "hasSprites" };
            }
        }
    }
}
