namespace AutoApiGen.Internal.Models;

internal record ControllerModel(
    string Name,
    IImmutableList<MethodModel> Methods
);
