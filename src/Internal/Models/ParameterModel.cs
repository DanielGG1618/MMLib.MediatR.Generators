namespace AutoApiGen.Internal.Models;

internal record ParameterModel(
    string Name,
    string TypeName,
    IImmutableList<string> AttributeNames
);
