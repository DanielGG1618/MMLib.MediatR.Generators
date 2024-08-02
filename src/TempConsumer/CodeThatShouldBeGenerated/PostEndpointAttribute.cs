using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;

namespace TempConsumer.CodeThatShouldBeGenerated;

public class PostEndpointAttribute : EndpointAttribute
{
    public override required string Route { get; init; }
}
