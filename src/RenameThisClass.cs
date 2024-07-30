namespace AutoApiGen;

public static class RenameThisClass
{
    public static ISet<string> EndpointAttributeNames { get; } = new HashSet<string>
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
