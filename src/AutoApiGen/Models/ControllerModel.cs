namespace AutoApiGen.Models;

internal record ControllerModel(
    string Name,
    string BaseRoute,
    IImmutableList<MethodModel> Methods
);
