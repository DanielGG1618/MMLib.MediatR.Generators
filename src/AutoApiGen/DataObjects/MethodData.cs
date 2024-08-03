namespace AutoApiGen.DataObjects;

internal record MethodData(
    string HttpMethod,
    IImmutableList<string> Attributes,
    string Name,
    IImmutableList<ParameterData> Parameters,
    string RequestType,
    string ResponseType
);
