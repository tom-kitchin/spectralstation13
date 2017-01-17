namespace Components.Test
{
    public interface ITestComponent : IIdentifiedComponent
    {
        bool space { get; }
        string habit { get; }
        int counter { get; set; }
    }
}
