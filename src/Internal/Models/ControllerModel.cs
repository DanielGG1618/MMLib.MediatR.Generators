namespace AutoApiGen.Internal.Models;

internal record ControllerModel(
    string Namespace,
    string Name,
    IImmutableList<MethodModel> Methods
);
