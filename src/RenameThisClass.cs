namespace AutoApiGen;

public static class RenameThisClass
{
    public static ISet<string> EndpointAttributeNames { get; } = new HashSet<string>
    {
        "GetEndpointAttribute",
        "GetEndpoint",
        "PostEndpointAttribute",
        "PutEndpointAttribute",
        "DeleteEndpointAttribute",
        "PatchEndpointAttribute",
        "HeadEndpointAttribute",
        "OptionsEndpointAttribute",
        "TraceEndpointAttribute"
    };
}
