namespace TempConsumer.CodeThatShouldBeGenerated;

[AttributeUsage(AttributeTargets.Class)]
public abstract class EndpointAttribute : Attribute
{
    public abstract required string Route { get; init; }
}
