namespace AutoApiGen.DataObjects;

internal record ControllerData(
    string BaseRoute,
    string Name,
    IImmutableList<MethodData> Methods
);
