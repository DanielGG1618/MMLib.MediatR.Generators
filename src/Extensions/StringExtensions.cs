namespace AutoApiGen.Extensions;

public static class StringExtensions
{
    public static string ToControllerName(this string controllerName)
        => controllerName.EndsWith("Controller", StringComparison.OrdinalIgnoreCase)
            ? controllerName 
            : $"{controllerName}Controller";
}