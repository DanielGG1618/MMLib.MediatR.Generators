namespace AutoApiGen.Wrappers;

public class RouteString
{
    private readonly string _string;
    private readonly IImmutableList<string> _parts;

    public string BaseRoute => _parts[0];
    
    public static RouteString Wrap(string @string) => new(@string);
    
    public static implicit operator string(RouteString routeString) => routeString._string;
    
    private RouteString(string @string)
    {
        _string = @string;
        _parts = _string.Split('/').ToImmutableArray();
    }
}
