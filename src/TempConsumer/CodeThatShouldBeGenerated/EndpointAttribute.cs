namespace TempConsumer.CodeThatShouldBeGenerated;

public abstract class EndpointAttribute : Attribute
{
    public abstract required string Route { get; init; }
}
