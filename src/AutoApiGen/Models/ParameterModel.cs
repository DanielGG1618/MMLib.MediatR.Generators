namespace AutoApiGen.Models;

internal record ParameterModel(
    string Name,
    string TypeName,
    IImmutableList<string> AttributeNames
);
