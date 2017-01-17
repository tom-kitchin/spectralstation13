namespace Components.Control
{
    public interface IIsPlayerControllableComponent : IIdentifiedComponent
    {
        bool currentlyControlled { get; set; }
    }
}
