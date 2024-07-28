namespace AutoApiGen.Internal.Models;

internal partial record MethodModel(
    string Name,
    string HttpMethod,
    string RequestType,
    string ResponseType,
    string Attributes,
    IImmutableList<ParameterModel> Parameters,
    IImmutableList<string> RequestProperties,
    string? Comment = ""
);
