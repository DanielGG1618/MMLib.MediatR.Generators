using System.Collections.Generic;

namespace AutoApiGen.Internal.Models;

internal partial record ControllerModel
{
    public string Namespace { get; init; } = null!;

    public string Name { get; init; } = null!;

    public IEnumerable<MethodModel> Methods { get; init; } = [];
}
