namespace AutoApiGen;

internal static class RenameThisClass
{
    public static ISet<string> EndpointAttributeNames { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "GetEndpoint",
        "PostEndpoint",
        "PutEndpoint",
        "DeleteEndpoint",
        "PatchEndpoint",
        "HeadEndpoint",
        "OptionsEndpoint",
        "TraceEndpoint"
    };
}
