namespace TempConsumer;

[GetEndpoint(Route = "Users")]
public class A;

public class GetEndpointAttribute : EndpointAttribute
{
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public required string Route { get; init; }
}

public class EndpointAttribute : Attribute;
