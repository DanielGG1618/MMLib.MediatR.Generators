namespace AutoApiGen.Internal.Models;

internal record ControllerModel(
    string Name,
    string BaseRoute,
    IImmutableList<MethodModel> Methods
);
