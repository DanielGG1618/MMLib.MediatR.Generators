namespace AutoApiGen.Controllers.Internal.Models;

internal record ParameterModel(
    string Name,
    string Type,
    string Attribute = "",
    bool CanPostInitiateCommand = false
);
