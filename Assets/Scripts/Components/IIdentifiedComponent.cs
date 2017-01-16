using Svelto.ES;

namespace Components
{
    public interface IIdentifiedComponent : IComponent
    {
        string ComponentIdentifier { get; }
    }
}
