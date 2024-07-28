using System.Collections.Generic;

namespace AutoApiGen.Internal.Models;

internal partial record MethodModel
{
    public required string Name { get; init; }

    public string? Template { get; init; }

    public required string HttpMethod { get; init; }

    public string? ResponseType { get; init; }

    public string? Comment { get; init; }

    public string? Attributes { get; set; }

    public List<ParameterModel> Parameters { get; init; } = [];

    public string? RequestType { get; init; }

    public List<string> RequestProperties { get; init; } = [];
}