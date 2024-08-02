namespace AutoApiGen.Internal.Models;

internal record MethodModel(
    string Name,
    string HttpMethod,
    string RequestType,
    string ResponseType,
    string Attributes,
    IImmutableList<ParameterModel> Parameters,
    IImmutableList<string> RequestProperties
);
