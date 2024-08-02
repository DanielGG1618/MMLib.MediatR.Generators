using System.Diagnostics.CodeAnalysis;

namespace TempConsumer.CodeThatShouldBeGenerated;

public class GetEndpointAttribute : EndpointAttribute
{
    public override required string Route { get; init; }
}