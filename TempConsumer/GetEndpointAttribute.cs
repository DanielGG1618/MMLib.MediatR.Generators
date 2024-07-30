namespace TempConsumer;

[GetEndpoint(nameof(A))]
public class A;

public class GetEndpointAttribute(string a) : Attribute
{
    public string A { get; } = a;

    public string B { get; } = a + 2;
}
